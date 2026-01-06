namespace ComplianceApi.Models.DTOs
{
    /// <summary>
    /// Request gửi tới AI Service để phân tích tài liệu
    /// </summary>
    public class AIAnalysisRequest
    {
        public string DocumentId { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Response từ AI Service sau khi phân tích
    /// </summary>
    public class AIAnalysisResponse
    {
        public string DocumentId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int? ComplianceScore { get; set; }
        public List<ComplianceIssue>? Issues { get; set; }
        public string? Summary { get; set; }
        public DateTime AnalyzedAt { get; set; }
    }

    /// <summary>
    /// Chi tiết vấn đề tuân thủ được phát hiện
    /// </summary>
    public class ComplianceIssue
    {
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public int? LineNumber { get; set; }
    }
}
