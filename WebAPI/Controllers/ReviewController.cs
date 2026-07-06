using Core.Entities;
using Core.RepositoryInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebAPI.DTOs.Review;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        public ReviewController(IUnitOfWork uow) => _uow = uow;
        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // 👀 للمريض: مراجعات ممرض معيّن + المتوسط (عام)
        [HttpGet("Nurse/{nurseId}")]
        public async Task<IActionResult> ForNurse(string nurseId)
        {
            var reviews = await _uow.Reviews.GetByNurseAsync(nurseId);
            return Ok(new
            {
                nurseId,
                average = reviews.Any() ? Math.Round(reviews.Average(r => r.Rate), 1) : 0,
                count = reviews.Count,
                items = reviews.Select(Map)
            });
        }

        // 👩‍⚕️ للممرض: المراجعات اللي وصلتله
        [Authorize(Roles = "Nurse")]
        [HttpGet("Received")]
        public async Task<IActionResult> Received()
        {
            var reviews = await _uow.Reviews.GetByNurseAsync(UserId);
            return Ok(new
            {
                average = reviews.Any() ? Math.Round(reviews.Average(r => r.Rate), 1) : 0,
                count = reviews.Count,
                items = reviews.Select(Map)
            });
        }

        // 🧑 للمريض: سجل مراجعاتي
        [Authorize(Roles = "Patient")]
        [HttpGet("My-Reviews")]
        public async Task<IActionResult> MyReviews()
            => Ok((await _uow.Reviews.GetByPatientAsync(UserId)).Select(Map));

        // ➕ إضافة تقييم (واحد بس لكل ممرض)
        [Authorize(Roles = "Patient")]
        [HttpPost]
        public async Task<IActionResult> Add(AddReviewDto dto)
        {
            if (dto.Rate is < 1 or > 5) return BadRequest("التقييم من 1 لـ 5");

            var nurse = await _uow.Nurses.GetByIdAsync(dto.NurseId);
            if (nurse is null) return NotFound("Nurse not found");

            var existing = await _uow.Reviews.GetByPatientAndNurseAsync(UserId, dto.NurseId);
            if (existing is not null)
                return BadRequest("عملت تقييم للممرض ده قبل كده، تقدر تعدّله بس");

            await _uow.Reviews.AddAsync(new Review
            {
                PatientId = UserId,
                NurseId = dto.NurseId,
                Rate = dto.Rate,
                Description = dto.Description,
                Date = DateTime.UtcNow
            });
            await _uow.CompleteAsync();
            return Ok("تم إضافة التقييم");
        }

        // ✏️ تعديل تقييمي
        [Authorize(Roles = "Patient")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(string id, UpdateReviewDto dto)
        {
            if (dto.Rate is < 1 or > 5) return BadRequest("التقييم من 1 لـ 5");
            var review = await _uow.Reviews.GetByIdAsync(id);
            if (review is null) return NotFound();
            if (review.PatientId != UserId) return Forbid();

            review.Rate = dto.Rate;
            review.Description = dto.Description;
            review.Date = DateTime.UtcNow;
            _uow.Reviews.Update(review);
            await _uow.CompleteAsync();
            return Ok("تم تعديل التقييم");
        }

        // 🗑️ حذف تقييمي
        [Authorize(Roles = "Patient")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var review = await _uow.Reviews.GetByIdAsync(id);
            if (review is null) return NotFound();
            if (review.PatientId != UserId) return Forbid();
            _uow.Reviews.Delete(review);
            await _uow.CompleteAsync();
            return Ok("تم حذف التقييم");
        }

        private static object Map(Review r) => new
        { r.Id, r.NurseId, r.PatientId, r.Rate, r.Description, r.Date };
    }
}
