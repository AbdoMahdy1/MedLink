using Core.Entities;
using Core.RepositoryInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController: ControllerBase
    {
        private readonly IReviewRepository reviewRepo;

        public ReviewController(IReviewRepository reviewRepository)
        {
            reviewRepo = reviewRepository;
        }

        //[HttpPost("Add-Review")]
        //public IActionResult Create(Review review)
        //{
        //    if (review != null)
        //    {
        //        reviewRepo.Add(review);
        //        return CreatedAtAction(nameof(GetById), new { id = review.Id }, review);
        //    }
        //    return BadRequest();
        //}
    }
}
