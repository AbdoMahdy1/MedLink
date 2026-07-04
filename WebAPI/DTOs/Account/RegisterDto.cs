using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs.Account
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }
        
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password id required")]
        public string Password { get; set; }
    }
}
