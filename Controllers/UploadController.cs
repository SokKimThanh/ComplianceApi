using Microsoft.AspNetCore.Mvc;
using ComplianceApi.Data;
using ComplianceApi.Models;

namespace ComplianceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        // Danh sách các định dạng hình ảnh bị chặn
        private static readonly HashSet<string> BlockedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".svg", ".webp", ".ico", ".tiff", ".tif", ".heic", ".heif"
        };

        public UploadController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("upload-local")]
        public async Task<IActionResult> UploadLocal(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File không hợp lệ.");

            // Kiểm tra định dạng file
            var fileExtension = Path.GetExtension(file.FileName);
            if (BlockedImageExtensions.Contains(fileExtension))
            {
                return BadRequest(new
                {
                    Message = "File hình ảnh không được phép tải lên.",
                    BlockedExtension = fileExtension,
                    AllowedTypes = "Chỉ chấp nhận file tài liệu (PDF, DOCX, XLSX, TXT, v.v.)"
                });
            }

            var storagePath = _configuration["FileStorage:LocalPath"] ?? "Uploads";
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), storagePath);

            if (!Directory.Exists(fullPath)) Directory.CreateDirectory(fullPath);

            var fileId = Guid.NewGuid();
            var fileName = $"{fileId}{Path.GetExtension(file.FileName)}";
            var savePath = Path.Combine(fullPath, fileName);

            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var document = new Document
            {
                Id = fileId,
                UserId = Guid.Parse("00000000-0000-0000-0000-000000000000"), // sau này thay bằng JWT
                FileName = file.FileName,
                FilePath = savePath,
                Status = "Pending",
                UploadedAt = DateTime.UtcNow
            };

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Tải lên thành công!",
                DocumentId = document.Id,
                StoredPath = savePath
            });
        }
    }
}
