using Core.Entities;
using Core.RepositoryInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs.SystemService;
using WebAPI.Services;
using static System.Net.WebRequestMethods;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SystemServiceController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IFileService _files;
        public SystemServiceController(IUnitOfWork uow, IFileService files)
        {
            _uow = uow;
            _files = files;
        }

        [HttpGet("Get-All")]
        public async Task<IActionResult> GetAll() => Ok(await _uow.SystemServices.GetAllAsync());

        [Authorize(Roles = "Admin")]
        [HttpPost("Add")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Add([FromForm] AddSystemServiceDto dto)
        {
            var service = new SystemService
            {
                Name = dto.Name,
                Description = dto.Description,
                IsDefault = dto.IsDefault,
                IsCareService = dto.IsCareService,
                IsFixedPrice = dto.IsFixedPrice,
                MinPrice = dto.MinPrice,
                MaxPrice = dto.MaxPrice,
                FixedPrice = dto.FixedPrice
            };
            if (dto.Image is not null)
                service.ImageUrl = await _files.SaveAsync(dto.Image, "services");

            await _uow.SystemServices.AddAsync(service);
            await _uow.CompleteAsync(); // عشان ياخد Id

            // وزّع الخدمة على كل ممرض لسه مش واخدها
            var missing = await _uow.NurseServices.GetNurseIdsMissingServiceAsync(service.Id);
            await _uow.NurseServices.AddRangeAsync(missing.Select(nid => new NurseService
            {
                NurseId = nid,
                SystemServiceId = service.Id,
                Price = service.IsFixedPrice ? service.FixedPrice : service.MinPrice
            }));
            await _uow.CompleteAsync();

            return Ok($"Service added and assigned to {missing.Count} nurses");
        }
    }
}
