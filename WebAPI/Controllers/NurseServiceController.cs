using Core.Entities;
using Core.RepositoryInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebAPI.DTOs.NurseService;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Nurse")]
    public class NurseServiceController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        public NurseServiceController(IUnitOfWork uow) => _uow = uow;

        private string NurseId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // خدماتي
        [HttpGet("My-Services")]
        public async Task<IActionResult> MyServices()
        {
            var services = await _uow.NurseServices.GetByNurseAsync(NurseId);
            return Ok(services.Select(ns => new
            {
                ns.Id,
                ServiceName = ns.SystemService.Name,
                ns.SystemService.IsFixedPrice,
                ns.SystemService.MinPrice,
                ns.SystemService.MaxPrice,
                ns.Price,
                ns.SystemService.IsCareService
            }));
        }

        // الممرض يضيف خدمة اختيارية لنفسه (زي الرعاية)
        [HttpPost("Add")]
        public async Task<IActionResult> Add(AddNurseServiceDto dto)
        {
            var sys = await _uow.SystemServices.GetByIdAsync(dto.SystemServiceId);
            if (sys is null) return NotFound("System service not found");

            var mine = await _uow.NurseServices.GetByNurseAsync(NurseId);
            if (mine.Any(ns => ns.SystemServiceId == sys.Id))
                return BadRequest("انت بتقدّم الخدمة دي بالفعل");

            var price = ResolvePrice(sys, dto.Price, out var err);
            if (err is not null) return BadRequest(err);

            await _uow.NurseServices.AddAsync(new NurseService
            { NurseId = NurseId, SystemServiceId = sys.Id, Price = price });
            await _uow.CompleteAsync();
            return Ok("تمت إضافة الخدمة");
        }

        // الممرض يعدّل سعره (في حدود الـ Min/Max)
        [HttpPut("Update-Price/{nurseServiceId}")]
        public async Task<IActionResult> UpdatePrice(string nurseServiceId, UpdateNurseServicePriceDto dto)
        {
            var ns = await _uow.NurseServices.GetWithDetailsAsync(nurseServiceId);
            if (ns is null) return NotFound("Service not found");
            if (ns.NurseId != NurseId) return Forbid();

            var price = ResolvePrice(ns.SystemService, dto.Price, out var err);
            if (err is not null) return BadRequest(err);

            ns.Price = price;
            _uow.NurseServices.Update(ns);
            await _uow.CompleteAsync();
            return Ok("تم تعديل السعر");
        }

        // الممرض يشيل خدمة اختيارية من عنده
        [HttpDelete("{nurseServiceId}")]
        public async Task<IActionResult> Remove(string nurseServiceId)
        {
            var ns = await _uow.NurseServices.GetWithDetailsAsync(nurseServiceId);
            if (ns is null) return NotFound("Service not found");
            if (ns.NurseId != NurseId) return Forbid();

            _uow.NurseServices.Delete(ns);
            await _uow.CompleteAsync();
            return Ok("تم حذف الخدمة");
        }

        // السعر: لو ثابت بياخد FixedPrice، لو رينج لازم يكون بين Min و Max
        private static decimal ResolvePrice(SystemService sys, decimal requested, out string? error)
        {
            error = null;
            if (sys.IsFixedPrice) return sys.FixedPrice;
            if (requested < sys.MinPrice || requested > sys.MaxPrice)
            { error = $"السعر لازم يكون بين {sys.MinPrice} و {sys.MaxPrice}"; return 0; }
            return requested;
        }
    }
}
