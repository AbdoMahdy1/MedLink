using Core.Common;
using Core.Entities;
using Core.RepositoryInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Security.Claims;
using WebAPI.DTOs.Order;

namespace WebAPI.Controllers

{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        public OrderController(IUnitOfWork uow) => _uow = uow;

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // ---------- إنشاء أوردر (المريض) ----------
        [Authorize(Roles = "Patient")]
        [HttpPost("Add-Order")]
        public async Task<IActionResult> Create(AddOrderDto dto)
        {
            var nurseService = await _uow.NurseServices.GetWithDetailsAsync(dto.NurseServiceId);
            if (nurseService is null) return NotFound("Nurse service not found");

            // الممرض لازم يكون مقبول
            var nurse = await _uow.Nurses.GetByIdAsync(nurseService.NurseId);
            if (nurse is null || nurse.Status != NurseStatus.Approved)
                return BadRequest("الممرض غير متاح للحجز");

            // 🗓️ فحص الجدول (الرعاية + حد الـ 15)
            var isCare = nurseService.SystemService.IsCareService;
            var (ok, reason) = await NurseScheduleChecker.CheckAsync(
                _uow, nurseService.NurseId, dto.OrderTime, isCare);
            if (!ok) return Conflict(reason);   // 409

            var order = new Order
            {
                PatientId = UserId,
                NurseId = nurseService.NurseId,
                NurseServiceId = nurseService.Id,
                Date = dto.OrderTime,
                ServiceType = dto.ServiceType,
                Address = dto.Address,
                Description = dto.Description ?? "",
                PatientAge = dto.PatientAge,
                Price = nurseService.Price,
                Status = OrderStatus.Pending
            };

            await _uow.Orders.AddAsync(order);
            await _uow.CompleteAsync();
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }

        // ---------- أوردر واحد ----------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var order = await _uow.Orders.GetWithDetailsAsync(id);
            if (order is null) return NotFound();
            // المريض أو الممرض صاحب الأوردر بس
            if (order.PatientId != UserId && order.NurseId != UserId && !User.IsInRole("Admin"))
                return Forbid();
            return Ok(order);
        }

        // ---------- أوردرات المريض ----------
        [Authorize(Roles = "Patient")]
        [HttpGet("My-Orders")]
        public async Task<IActionResult> MyOrders()
            => Ok(await _uow.Orders.GetByPatientAsync(UserId));

        // ---------- أوردرات الممرض ----------
        [Authorize(Roles = "Nurse")]
        [HttpGet("Nurse-Orders")]
        public async Task<IActionResult> NurseOrders()
            => Ok(await _uow.Orders.GetByNurseAsync(UserId));

        // ---------- الممرض يغيّر الحالة (Accept/Reject/Complete) ----------
        [Authorize(Roles = "Nurse")]
        [HttpPut("Update-Status/{id}")]
        public async Task<IActionResult> UpdateStatus(string id, UpdateOrderStatusDto dto)
        {
            var order = await _uow.Orders.GetByIdAsync(id);
            if (order is null) return NotFound();
            if (order.NurseId != UserId) return Forbid();

            var allowed = new[] { OrderStatus.Accepted, OrderStatus.Rejected, OrderStatus.Completed };
            if (!allowed.Contains(dto.Status))
                return BadRequest("الحالة غير صحيحة");

            order.Status = dto.Status;
            _uow.Orders.Update(order);
            await _uow.CompleteAsync();
            return Ok($"تم تحديث الحالة إلى {dto.Status}");
        }

        // ---------- المريض يلغي الأوردر ----------
        [Authorize(Roles = "Patient")]
        [HttpPut("Cancel/{id}")]
        public async Task<IActionResult> Cancel(string id)
        {
            var order = await _uow.Orders.GetByIdAsync(id);
            if (order is null) return NotFound();
            if (order.PatientId != UserId) return Forbid();
            if (order.Status == OrderStatus.Completed)
                return BadRequest("مينفعش تلغي أوردر مكتمل");

            order.Status = OrderStatus.Cancelled;
            _uow.Orders.Update(order);
            await _uow.CompleteAsync();
            return Ok("تم إلغاء الأوردر");
        }
    }
}
