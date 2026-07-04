namespace WebAPI.DTOs.Profile
{
    public class UpdateNurseProfileDto
    {
        public string? Name { get; set; }
        public int? Age { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public string? Description { get; set; }
        public int? ExperienceYears { get; set; }
        public IFormFile? ProfilePhoto { get; set; }
    }
}
