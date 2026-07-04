using Core.Entities;
using Core.RepositoryInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebAPI.DTOs.Profile;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly UserManager<AppUser> _users;
        private readonly IFileService _files;

        public ProfileController(IUnitOfWork uow, UserManager<AppUser> users, IFileService files)
        { _uow = uow; _users = users; _files = files; }

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // ---------- عرض بروفايلي (حسب الدور) ----------
        [HttpGet("Me")]
        public async Task<IActionResult> Me()
        {
            var user = await _users.FindByIdAsync(UserId);
            if (user is null) return NotFound();
            var roles = await _users.GetRolesAsync(user);

            if (roles.Contains("Nurse"))
            {
                var n = await _uow.Nurses.GetWithDetailsAsync(UserId);
                if (n is null) return NotFound();
                return Ok(new
                {
                    role = "Nurse",
                    n.Id,
                    user.Email,
                    user.PhoneNumber,
                    n.Name,
                    n.Age,
                    n.Address,
                    n.Gender,
                    n.Description,
                    n.ExperienceYears,
                    n.ImageUrl,
                    n.CvFiles,
                    Status = n.Status.ToString(),
                    Services = n.NurseServices.Select(s => new { s.Id, s.SystemService.Name, s.Price })
                });
            }
            if (roles.Contains("Patient"))
            {
                var p = await _uow.Patients.GetByIdAsync(UserId);
                if (p is null) return NotFound();
                return Ok(new
                {
                    role = "Patient",
                    p.Id,
                    user.Email,
                    user.PhoneNumber,
                    p.Name,
                    p.Age,
                    p.Address,
                    p.Gender,
                    p.ImageUrl
                });
            }
            return Ok(new { role = roles.FirstOrDefault(), user.Id, user.Email, user.PhoneNumber });
        }

        // ---------- تعديل بروفايل المريض ----------
        [Authorize(Roles = "Patient")]
        [HttpPut("Patient")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdatePatient([FromForm] UpdatePatientProfileDto dto)
        {
            var p = await _uow.Patients.GetByIdAsync(UserId);
            if (p is null) return NotFound();

            p.Name = dto.Name ?? p.Name;
            p.Age = dto.Age ?? p.Age;
            p.Address = dto.Address ?? p.Address;
            p.Gender = dto.Gender ?? p.Gender;

            if (dto.ProfilePhoto is not null)
            {
                _files.Delete(p.ImageUrl);                              // امسح القديمة
                p.ImageUrl = await _files.SaveAsync(dto.ProfilePhoto, "patients");
            }

            _uow.Patients.Update(p);
            await _uow.CompleteAsync();
            return Ok("تم تحديث البيانات");
        }

        // ---------- تعديل بروفايل الممرض ----------
        [Authorize(Roles = "Nurse")]
        [HttpPut("Nurse")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateNurse([FromForm] UpdateNurseProfileDto dto)
        {
            var n = await _uow.Nurses.GetByIdAsync(UserId);
            if (n is null) return NotFound();

            n.Name = dto.Name ?? n.Name;
            n.Age = dto.Age ?? n.Age;
            n.Address = dto.Address ?? n.Address;
            n.Gender = dto.Gender ?? n.Gender;
            n.Description = dto.Description ?? n.Description;
            n.ExperienceYears = dto.ExperienceYears ?? n.ExperienceYears;

            if (dto.ProfilePhoto is not null)
            {
                _files.Delete(n.ImageUrl);
                n.ImageUrl = await _files.SaveAsync(dto.ProfilePhoto, "nurses");
            }

            _uow.Nurses.Update(n);
            await _uow.CompleteAsync();
            return Ok("تم تحديث البيانات");
        }

        // ---------- الممرض يضيف ملف CV بعد التسجيل ----------
        [Authorize(Roles = "Nurse")]
        [HttpPost("Nurse/Cv")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddCv(IFormFile file)
        {
            var n = await _uow.Nurses.GetByIdAsync(UserId);
            if (n is null) return NotFound();

            var url = await _files.SaveAsync(file, "cvs", allowDocuments: true);
            n.CvFiles.Add(url);
            _uow.Nurses.Update(n);
            await _uow.CompleteAsync();
            return Ok(new { url, files = n.CvFiles });
        }

        // ---------- الممرض يحذف ملف CV ----------
        [Authorize(Roles = "Nurse")]
        [HttpDelete("Nurse/Cv")]
        public async Task<IActionResult> RemoveCv([FromQuery] string fileUrl)
        {
            var n = await _uow.Nurses.GetByIdAsync(UserId);
            if (n is null) return NotFound();
            if (!n.CvFiles.Remove(fileUrl)) return NotFound("الملف مش موجود");

            _files.Delete(fileUrl);
            _uow.Nurses.Update(n);
            await _uow.CompleteAsync();
            return Ok(new { files = n.CvFiles });
        }
    }
}
