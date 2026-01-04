using Microsoft.EntityFrameworkCore;
using ComplianceApi.Models; // namespace chứa Document và Report

namespace ComplianceApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Document> Documents { get; set; }
        public DbSet<Report> Reports { get; set; }
    }
}
