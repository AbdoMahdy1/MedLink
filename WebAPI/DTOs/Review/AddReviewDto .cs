namespace WebAPI.DTOs.Review
{
    public class AddReviewDto
    {
        public string NurseId { get; set; }
        public int Rate { get; set; }
        public string? Description { get; set; }
    }
}
