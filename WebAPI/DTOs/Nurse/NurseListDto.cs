namespace WebAPI.DTOs.Nurse
{
    public class NurseListDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public int ExperienceYears { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string Status { get; set; }
        public List<NurseServiceDto> Services { get; set; } = new();
    }
    public class NurseServiceDto
    {
        public string ServiceName { get; set; }
        public decimal Price { get; set; }
    }


}
