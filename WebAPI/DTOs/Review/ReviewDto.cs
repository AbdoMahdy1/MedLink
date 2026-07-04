namespace WebAPI.DTOs.Review
{
    public class ReviewDto
    {
        public string? Id { get; set; }
        public string PatientId { get; set; }
        public string NurseId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
