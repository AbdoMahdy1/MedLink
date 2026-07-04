using Core.Entities;
using Core.RepositoryInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebAPI.DTOs.Nurse;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NurseController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        public NurseController(IUnitOfWork uow) => _uow = uow;

        // المرضى يشوفوا الممرضين المقبولين بس
        [HttpGet("Get-All")]
        public async Task<IActionResult> GetAll()
        {
            var nurses = await _uow.Nurses.GetAllWithDetailsAsync(NurseStatus.Approved);
            var dto = nurses.Select(MapToDto).ToList();
            return Ok(dto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var nurse = await _uow.Nurses.GetWithDetailsAsync(id);
            if (nurse is null) return NotFound("Nurse not found");
            return Ok(MapToDto(nurse));
        }

        // الممرض يحدّث سعر خدمة من خدماته (مع التأكد إن السعر داخل الرينج)
        [Authorize(Roles = "Nurse")]
        [HttpPut("Update-Service-Price/{nurseServiceId}")]
        public async Task<IActionResult> UpdatePrice(string nurseServiceId, [FromBody] decimal price)
        {
            var meId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ns = await _uow.NurseServices.GetWithDetailsAsync(nurseServiceId);
            if (ns is null) return NotFound("Service not found");
            if (ns.NurseId != meId) return Forbid();

            if (ns.SystemService.IsFixedPrice)
                price = ns.SystemService.FixedPrice; // سعر ثابت → مينفعش يغيره
            else if (price < ns.SystemService.MinPrice || price > ns.SystemService.MaxPrice)
                return BadRequest($"Price must be between {ns.SystemService.MinPrice} and {ns.SystemService.MaxPrice}");

            ns.Price = price;
            _uow.NurseServices.Update(ns);
            await _uow.CompleteAsync();
            return Ok("Price updated");
        }

        private static NurseListDto MapToDto(Nurse n) => new()
        {
            Id = n.Id,
            Name = n.Name,
            Email = n.User?.Email!,
            PhoneNumber = n.User?.PhoneNumber!,
            Age = n.Age,
            Gender = n.Gender,
            ExperienceYears = n.ExperienceYears,
            Description = n.Description,
            ImageUrl = n.ImageUrl,
            Status = n.Status.ToString(),
            Services = n.NurseServices.Select(ns => new NurseServiceDto
            { ServiceName = ns.SystemService.Name, Price = ns.Price }).ToList()
        };
    }
}
