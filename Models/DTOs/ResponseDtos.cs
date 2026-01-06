namespace ComplianceApi.Models.DTOs
{
    /// <summary>
    /// Response ch?a thông tin chi ti?t document
    /// </summary>
    public class DocumentResponseDto
    {
        /// <summary>
        /// ID duy nh?t c?a document
        /// </summary>
        public Guid DocumentId { get; set; }

        /// <summary>
        /// Tên file g?c
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// ???ng d?n file trên server
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// Tr?ng thái x? lý: Pending, Analyzed
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// ID c?a user ?ã upload
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Th?i gian upload (UTC)
        /// </summary>
        public DateTime UploadedAt { get; set; }

        /// <summary>
        /// File có t?n t?i trên disk không
        /// </summary>
        public bool FileExists { get; set; }
    }

    /// <summary>
    /// Response khi upload document thành công
    /// </summary>
    public class UploadResponseDto
    {
        /// <summary>
        /// Thông báo k?t qu?
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// ID c?a document ?ã upload
        /// </summary>
        public Guid DocumentId { get; set; }

        /// <summary>
        /// Tên file g?c
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// ???ng d?n l?u tr?
        /// </summary>
        public string StoredPath { get; set; } = string.Empty;

        /// <summary>
        /// Tr?ng thái: Pending ho?c Analyzed
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Th?i gian upload
        /// </summary>
        public DateTime UploadedAt { get; set; }

        /// <summary>
        /// K?t qu? phân tích AI (n?u có)
        /// </summary>
        public AIAnalysisResponse? AIAnalysis { get; set; }

        /// <summary>
        /// Ghi chú h??ng d?n
        /// </summary>
        public string? Note { get; set; }
    }

    /// <summary>
    /// Response khi phân tích document
    /// </summary>
    public class AnalyzeResponseDto
    {
        /// <summary>
        /// Thông báo k?t qu?
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// ID c?a document
        /// </summary>
        public Guid DocumentId { get; set; }

        /// <summary>
        /// Tên file
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Tr?ng thái sau phân tích
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// K?t qu? phân tích AI
        /// </summary>
        public AIAnalysisResponse AIAnalysis { get; set; } = new();
    }

    /// <summary>
    /// Response l?i chu?n
    /// </summary>
    public class ErrorResponseDto
    {
        /// <summary>
        /// Thông báo l?i
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Chi ti?t l?i (ch? hi?n th? trong Development)
        /// </summary>
        public string? Error { get; set; }

        /// <summary>
        /// D? li?u b? sung (tùy ch?n)
        /// </summary>
        public object? Data { get; set; }
    }
}
