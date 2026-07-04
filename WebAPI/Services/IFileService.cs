namespace WebAPI.Services
{
    public interface IFileService
    {
        Task<string> SaveAsync(IFormFile file, string subFolder, bool allowDocuments = false);
        void Delete(string? fileUrl);
    }
}
