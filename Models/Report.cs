using System;

namespace ComplianceApi.Models
{
    public class Report
    {
        public Guid Id { get; set; }
        public Guid DocumentId { get; set; }
        public int ComplianceScore { get; set; }
        public string Status { get; set; } = string.Empty;
        public string AIResultJson { get; set; } = string.Empty;

        public DateTime GeneratedAt { get; set; }
    }
}
