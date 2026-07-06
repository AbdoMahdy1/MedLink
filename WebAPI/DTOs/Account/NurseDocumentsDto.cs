namespace WebAPI.DTOs.Account
{
    public class NurseDocumentsDto
    {
        public IFormFile? ProfilePhoto { get; set; }
        public List<IFormFile>? CvFiles { get; set; } = new();
    }
}
