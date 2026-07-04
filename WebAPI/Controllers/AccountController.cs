using Core.Entities;
using Core.RepositoryInterfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPI.DTOs.Account;
using WebAPI.Services;
using static System.Net.WebRequestMethods;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _users;
        private readonly SignInManager<AppUser> _signIn;
        private readonly IUnitOfWork _uow;
        private readonly IFileService _files;

        public AccountController(UserManager<AppUser> users, SignInManager<AppUser> signIn,
            IUnitOfWork uow, IFileService files)
        {
            _users = users;
            _signIn = signIn;
            _uow = uow;
            _files = files;
        }

        [HttpPost("Register-Patient")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> RegisterPatient([FromForm] RegisterPatientDto dto)
        {
            var user = new AppUser { UserName = dto.Email, Email = dto.Email };
            var res = await _users.CreateAsync(user, dto.Password);
            if (!res.Succeeded) return BadRequest(res.Errors);
            await _users.AddToRoleAsync(user, "Patient");

            var patient = new Patient
            {
                Id = user.Id,
                Name = dto.Name,
                Age = dto.Age,
                Address = dto.Address,
                Gender = dto.Gender
            };
            if (dto.ProfilePhoto is not null)
                patient.ImageUrl = await _files.SaveAsync(dto.ProfilePhoto, "patients");

            await _uow.Patients.AddAsync(patient);
            await _uow.CompleteAsync();
            return Ok("Patient registered successfully");
        }

        [HttpPost("Register-Nurse")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> RegisterNurse([FromForm] RegisterNurseDto dto)
        {
            var user = new AppUser { UserName = dto.Email, Email = dto.Email };
            var res = await _users.CreateAsync(user, dto.Password);
            if (!res.Succeeded) return BadRequest(res.Errors);
            await _users.AddToRoleAsync(user, "Nurse");

            var nurse = new Nurse
            {
                Id = user.Id,
                Name = dto.Name,
                Age = dto.Age,
                Address = dto.Address,
                Gender = dto.Gender,
                Description = dto.Description,
                ExperienceYears = dto.ExperienceYears,
                Status = NurseStatus.Pending
            };

            // صورة البروفايل
            if (dto.ProfilePhoto is not null)
                nurse.ImageUrl = await _files.SaveAsync(dto.ProfilePhoto, "nurses");

            // ملفات الـ CV (مسموح PDF/Word)
            if (dto.CvFiles is not null)
                foreach (var cv in dto.CvFiles)
                    nurse.CvFiles.Add(await _files.SaveAsync(cv, "cvs", allowDocuments: true));

            await _uow.Nurses.AddAsync(nurse);

            // ضيف الخدمات الـ Default تلقائي
            var defaults = await _uow.SystemServices.GetDefaultsAsync();
            await _uow.NurseServices.AddRangeAsync(defaults.Select(s => new NurseService
            {
                NurseId = nurse.Id,
                SystemServiceId = s.Id,
                Price = s.IsFixedPrice ? s.FixedPrice : s.MinPrice
            }));

            await _uow.CompleteAsync();
            return Ok("Nurse registered, pending approval");
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _users.FindByEmailAsync(dto.Email);
            if (user is null) return Unauthorized("Invalid credentials");

            // isPersistent: true → الكوكي تفضل بعد قفل المتصفح
            var result = await _signIn.PasswordSignInAsync(user, dto.Password, true, false);
            if (!result.Succeeded) return Unauthorized("Invalid credentials");

            var roles = await _users.GetRolesAsync(user);
            return Ok(new { user.Id, user.Email, Roles = roles });
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signIn.SignOutAsync();   // بيمسح الكوكي
            return Ok("Logged out");
        }
    }
}
