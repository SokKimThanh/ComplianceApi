using Microsoft.AspNetCore.Mvc;
using ComplianceApi.Data;
using ComplianceApi.Models;
using ComplianceApi.Models.DTOs;

namespace ComplianceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<UploadController> _logger;

        // Danh sách các định dạng hình ảnh bị chặn
        private static readonly HashSet<string> BlockedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".svg", ".webp", ".ico", ".tiff", ".tif", ".heic", ".heif"
        };

        public UploadController(
            ApplicationDbContext context, 
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            ILogger<UploadController> logger)
        {
            _context = context;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        /// <summary>
        /// Upload file tài liệu với tùy chọn phân tích AI ngay lập tức
        /// </summary>
        /// <param name="file">File tài liệu (PDF, DOCX, XLSX, TXT)</param>
        /// <param name="analyze">True: Phân tích AI ngay | False: Chỉ lưu file (default: true)</param>
        /// <returns>Thông tin file và kết quả phân tích AI (nếu analyze=true)</returns>
        [HttpPost("upload")]
        [ProducesResponseType(typeof(UploadResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UploadResponseDto>> UploadDocument(
            [FromForm] IFormFile file, 
            [FromQuery] bool analyze = true)
        {
            try
            {
                // 1. Validate file
                var validationError = ValidateFile(file);
                if (validationError != null)
                    return validationError;

                // 2. Lưu file vật lý và ghi metadata vào Database (LUÔN LUÔN LÀM)
                var document = await SaveFileToInternalStorageAsync(file);

                // 3. Phân tích AI (NẾU được yêu cầu và được cấu hình)
                AIAnalysisResponse? aiResult = null;
                
                if (analyze)
                {
                    _logger.LogInformation("AI analysis requested for document: {DocumentId}", document.Id);
                    aiResult = await RequestAIAnalysisAsync(document);
                }
                else
                {
                    _logger.LogInformation("AI analysis skipped for document: {DocumentId}", document.Id);
                }

                // 4. Trả về kết quả động dựa trên tham số analyze
                var response = new UploadResponseDto
                {
                    Message = analyze 
                        ? (aiResult != null ? "Tải lên và phân tích thành công!" : "Tải lên thành công! AI Service không khả dụng.")
                        : "Tải lên thành công! (Chưa phân tích AI)",
                    DocumentId = document.Id,
                    FileName = file.FileName,
                    StoredPath = document.FilePath,
                    Status = document.Status,
                    UploadedAt = document.UploadedAt,
                    AIAnalysis = aiResult,
                    Note = analyze 
                        ? "File đã được phân tích tự động" 
                        : "Sử dụng endpoint POST /api/Upload/{documentId}/analyze để phân tích sau"
                };
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file: {FileName}", file?.FileName);
                return StatusCode(500, new ErrorResponseDto
                { 
                    Message = "Lỗi server khi xử lý file", 
                    Error = ex.Message 
                });
            }
        }

        /// <summary>
        /// Phân tích lại tài liệu đã upload trước đó
        /// </summary>
        /// <param name="documentId">ID của document đã upload</param>
        /// <returns>Kết quả phân tích AI</returns>
        [HttpPost("{documentId}/analyze")]
        [ProducesResponseType(typeof(AnalyzeResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AnalyzeResponseDto>> AnalyzeDocument(Guid documentId)
        {
            try
            {
                // 1. Tìm document trong database
                var document = await _context.Documents.FindAsync(documentId);
                
                if (document == null)
                {
                    return NotFound(new ErrorResponseDto
                    { 
                        Message = "Không tìm thấy tài liệu",
                        Data = new { DocumentId = documentId }
                    });
                }

                // 2. Kiểm tra file có tồn tại không
                if (!System.IO.File.Exists(document.FilePath))
                {
                    return NotFound(new ErrorResponseDto
                    { 
                        Message = "File vật lý không tồn tại",
                        Data = new { FilePath = document.FilePath }
                    });
                }

                // 3. Gọi AI Service để phân tích
                var aiResult = await RequestAIAnalysisAsync(document);

                if (aiResult == null)
                {
                    return StatusCode(503, new ErrorResponseDto
                    { 
                        Message = "AI Service không khả dụng hoặc bị lỗi",
                        Data = new 
                        {
                            DocumentId = documentId,
                            Status = document.Status,
                            Suggestion = "Vui lòng thử lại sau hoặc kiểm tra cấu hình AIService:Enabled"
                        }
                    });
                }

                // 4. Trả về kết quả phân tích
                var response = new AnalyzeResponseDto
                {
                    Message = "Phân tích thành công!",
                    DocumentId = document.Id,
                    FileName = document.FileName,
                    Status = document.Status,
                    AIAnalysis = aiResult
                };
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing document: {DocumentId}", documentId);
                return StatusCode(500, new ErrorResponseDto
                { 
                    Message = "Lỗi server khi phân tích tài liệu", 
                    Error = ex.Message 
                });
            }
        }

        /// <summary>
        /// Lấy thông tin chi tiết của document
        /// </summary>
        /// <param name="documentId">ID của document</param>
        /// <returns>Thông tin document</returns>
        [HttpGet("{documentId}")]
        [ProducesResponseType(typeof(DocumentResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DocumentResponseDto>> GetDocument(Guid documentId)
        {
            var document = await _context.Documents.FindAsync(documentId);
            
            if (document == null)
            {
                return NotFound(new ErrorResponseDto
                { 
                    Message = "Không tìm thấy tài liệu",
                    Data = new { DocumentId = documentId }
                });
            }
            
            var response = new DocumentResponseDto
            {
                DocumentId = document.Id,
                FileName = document.FileName,
                FilePath = document.FilePath,
                Status = document.Status,
                UserId = document.UserId,
                UploadedAt = document.UploadedAt,
                FileExists = System.IO.File.Exists(document.FilePath)
            };
            
            return Ok(response);
        }

        #region Private Helper Methods

        /// <summary>
        /// Validate file input: null check, size check, extension check
        /// </summary>
        private ActionResult<UploadResponseDto>? ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new ErrorResponseDto { Message = "File không hợp lệ." });

            // Kiểm tra định dạng file
            var fileExtension = Path.GetExtension(file.FileName);
            if (BlockedImageExtensions.Contains(fileExtension))
            {
                return BadRequest(new ErrorResponseDto
                {
                    Message = "File hình ảnh không được phép tải lên.",
                    Data = new
                    {
                        BlockedExtension = fileExtension,
                        AllowedTypes = "Chỉ chấp nhận file tài liệu (PDF, DOCX, XLSX, TXT, v.v.)"
                    }
                });
            }

            // Optional: Kiểm tra kích thước file (10MB limit)
            const long maxFileSize = 10 * 1024 * 1024; // 10MB
            if (file.Length > maxFileSize)
            {
                return BadRequest(new ErrorResponseDto
                {
                    Message = "File quá lớn. Giới hạn 10MB.",
                    Data = new
                    {
                        FileSize = file.Length,
                        MaxSize = maxFileSize
                    }
                });
            }

            return null; // Valid
        }

        /// <summary>
        /// Lưu file vật lý vào InternalStorage và ghi metadata vào Database
        /// </summary>
        /// <param name="file">File upload</param>
        /// <returns>Document entity đã lưu</returns>
        private async Task<Document> SaveFileToInternalStorageAsync(IFormFile file)
        {
            // 1. Xác định đường dẫn lưu trữ
            var storagePath = _configuration["FileStorage:LocalPath"] ?? "Uploads";
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), storagePath);

            if (!Directory.Exists(fullPath))
                Directory.CreateDirectory(fullPath);

            // 2. Tạo tên file unique
            var fileId = Guid.NewGuid();
            var fileName = $"{fileId}{Path.GetExtension(file.FileName)}";
            var savePath = Path.Combine(fullPath, fileName);

            // 3. Lưu file vật lý
            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _logger.LogInformation("File saved to: {FilePath}", savePath);

            // 4. Tạo metadata trong Database
            var document = new Document
            {
                Id = fileId,
                UserId = Guid.Parse("00000000-0000-0000-0000-000000000000"), // TODO: Replace with JWT User ID
                FileName = file.FileName,
                FilePath = savePath,
                Status = "Pending",
                UploadedAt = DateTime.UtcNow
            };

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Document metadata saved: {DocumentId}", fileId);

            return document;
        }

        /// <summary>
        /// Gọi AI Service để phân tích tài liệu tuân thủ CCPA
        /// </summary>
        /// <param name="document">Document entity chứa thông tin file</param>
        /// <returns>Kết quả phân tích từ AI hoặc null nếu thất bại</returns>
        private async Task<AIAnalysisResponse?> RequestAIAnalysisAsync(Document document)
        {
            var aiServiceEnabled = _configuration.GetValue<bool>("AIService:Enabled", false);
            
            if (!aiServiceEnabled)
            {
                _logger.LogInformation("AI Service is disabled. Skipping analysis.");
                return null;
            }

            try
            {
                var client = _httpClientFactory.CreateClient("AIService");
                
                var requestData = new AIAnalysisRequest
                {
                    DocumentId = document.Id.ToString(),
                    FilePath = document.FilePath,
                    FileName = document.FileName
                };

                _logger.LogInformation("Sending analysis request to AI Service for document: {DocumentId}", document.Id);

                var response = await client.PostAsJsonAsync("/analyze", requestData);

                if (response.IsSuccessStatusCode)
                {
                    var aiResult = await response.Content.ReadFromJsonAsync<AIAnalysisResponse>();
                    
                    // Cập nhật status document sau khi phân tích thành công
                    document.Status = "Analyzed";
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Document {DocumentId} analyzed successfully with score: {Score}", 
                        document.Id, aiResult?.ComplianceScore);

                    return aiResult;
                }
                else
                {
                    _logger.LogWarning("AI Service returned status code: {StatusCode} for document: {DocumentId}", 
                        response.StatusCode, document.Id);
                    return null;
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to connect to AI Service. Document: {DocumentId}", document.Id);
                return null;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "AI Service request timeout. Document: {DocumentId}", document.Id);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error calling AI Service. Document: {DocumentId}", document.Id);
                return null;
            }
        }

        #endregion
    }
}
