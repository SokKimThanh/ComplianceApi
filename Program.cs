using ComplianceApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models; // FIX: Add this using directive

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Cấu hình Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo // FIX: Use OpenApiInfo from Microsoft.OpenApi.Models
    {
        Version = "v1",
        Title = "Compliance API",
        Description = "API quản lý tài liệu và kiểm tra tuân thủ",
        Contact = new OpenApiContact // FIX: Use OpenApiContact from Microsoft.OpenApi.Models
        {
            Name = "Support Team",
            Email = "support@complianceapi.com"
        }
    });
});

// 1. Lấy chuỗi kết nối từ appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Đăng ký DbContext với PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Kích hoạt Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Compliance API v1");
        options.RoutePrefix = string.Empty; // Mở Swagger UI tại root URL (https://localhost:xxxx/)
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();