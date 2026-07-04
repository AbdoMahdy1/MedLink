using Core.Entities;
using Core.RepositoryInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Moderator,Admin")]
    public class ModeratorController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        public ModeratorController(IUnitOfWork uow) => _uow = uow;

        // الممرضين المعلقين عشان يراجع الـ CV
        [HttpGet("Pending-Nurses")]
        public async Task<IActionResult> Pending()
        {
            var nurses = await _uow.Nurses.GetAllWithDetailsAsync(NurseStatus.Pending);
            return Ok(nurses.Select(n => new { n.Id, n.Name, n.ExperienceYears, n.CvFiles }));
        }

        [HttpPut("Approve/{nurseId}")]
        public async Task<IActionResult> Approve(string nurseId) => await SetStatus(nurseId, NurseStatus.Approved);

        [HttpPut("Reject/{nurseId}")]
        public async Task<IActionResult> Reject(string nurseId) => await SetStatus(nurseId, NurseStatus.Rejected);

        private async Task<IActionResult> SetStatus(string nurseId, NurseStatus status)
        {
            var nurse = await _uow.Nurses.GetByIdAsync(nurseId);
            if (nurse is null) return NotFound("Nurse not found");
            nurse.Status = status;
            _uow.Nurses.Update(nurse);
            await _uow.CompleteAsync();
            return Ok($"Nurse {status}");
        }
    }
}
