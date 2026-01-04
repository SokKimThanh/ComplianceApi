using System;

namespace ComplianceApi.Models
{
    public class Document
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
    }
}
