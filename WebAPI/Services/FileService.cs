namespace WebAPI.Services
{

    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _http;

        private static readonly string[] ImageExt = { ".jpg", ".jpeg", ".png", ".webp" };
        private static readonly string[] DocExt = { ".pdf", ".doc", ".docx" };
        private const long MaxBytes = 5 * 1024 * 1024; // 5 MB

        public FileService(IWebHostEnvironment env, IHttpContextAccessor http)
        { _env = env; _http = http; }

        public async Task<string> SaveAsync(IFormFile file, string subFolder, bool allowDocuments = false)
        {
            if (file is null || file.Length == 0) throw new ArgumentException("مفيش ملف اتبعت");
            if (file.Length > MaxBytes) throw new ArgumentException("الملف أكبر من 5 ميجا");

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowed = allowDocuments ? ImageExt.Concat(DocExt) : ImageExt;
            if (!allowed.Contains(ext)) throw new ArgumentException("نوع الملف غير مسموح");

            var root = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var folder = Path.Combine(root, "uploads", subFolder);
            Directory.CreateDirectory(folder);

            var name = $"{Guid.NewGuid()}{ext}";
            var fullPath = Path.Combine(folder, name);
            using (var stream = new FileStream(fullPath, FileMode.Create))
                await file.CopyToAsync(stream);

            var req = _http.HttpContext!.Request;
            return $"{req.Scheme}://{req.Host}/uploads/{subFolder}/{name}";
        }

        public void Delete(string? fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl)) return;
            var fileName = Path.GetFileName(fileUrl);
            var root = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploads = Path.Combine(root, "uploads");
            if (!Directory.Exists(uploads)) return;
            var path = Directory.GetFiles(uploads, fileName, SearchOption.AllDirectories).FirstOrDefault();
            if (path is not null && File.Exists(path)) File.Delete(path);
        }
    }
}

