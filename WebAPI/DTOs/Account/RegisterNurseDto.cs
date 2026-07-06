using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs.Account
{
    public class RegisterNurseDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public string? Description { get; set; }
        public int ExperienceYears { get; set; }
    }
}
