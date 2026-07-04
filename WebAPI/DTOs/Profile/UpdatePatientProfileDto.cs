namespace WebAPI.DTOs.Profile
{
    public class UpdatePatientProfileDto
    {
        public string? Name { get; set; }
        public int? Age { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public IFormFile? ProfilePhoto { get; set; }
    }
}
