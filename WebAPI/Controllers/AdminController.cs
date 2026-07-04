using Core.Entities;
using Core.RepositoryInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs.Account;
using WebAPI.DTOs.SystemService;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<AppUser> _users;
        public AdminController(UserManager<AppUser> users) => _users = users;

        [HttpPost("Add-Moderator")]
        public async Task<IActionResult> AddModerator(AddModeratorDto dto)
        {
            var user = new AppUser { UserName = dto.Email, Email = dto.Email, EmailConfirmed = true };
            var res = await _users.CreateAsync(user, dto.Password);
            if (!res.Succeeded) return BadRequest(res.Errors);
            await _users.AddToRoleAsync(user, "Moderator");
            return Ok("Moderator created");
        }
    }
}
