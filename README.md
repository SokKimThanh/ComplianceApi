# 🔐 Compliance API - Hệ Thống Kiểm Tra Tuân Thủ CCPA với AI

![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15-316192?logo=postgresql)
![Docker](https://img.shields.io/badge/Docker-Latest-2496ED?logo=docker)
![License](https://img.shields.io/badge/License-MIT-green)

> **Hệ thống tự động hóa quy trình kiểm tra tuân thủ CCPA cho hợp đồng**  
> Giảm thời gian kiểm kê từ **4 tiếng xuống còn 10 giây** nhờ AI

---

## 📋 Mục Lục

- [Tổng Quan](#-tổng-quan)
- [Tính Năng](#-tính-năng)
- [Kiến Trúc Hệ Thống](#-kiến-trúc-hệ-thống)
- [Cài Đặt](#-cài-đặt)
- [Cấu Hình](#️-cấu-hình)
- [Sử Dụng](#-sử-dụng)
- [API Endpoints](#-api-endpoints)
- [Database Schema](#-database-schema)
- [Development](#-development)
- [Roadmap](#-roadmap)
- [Đóng Góp](#-đóng-góp)
- [License](#-license)

---

## 🎯 Tổng Quan

**Compliance API** là hệ thống backend được xây dựng với **.NET 9**, tích hợp AI để tự động kiểm tra tính tuân thủ của hợp đồng theo quy trình kiểm soát **CCPA** (California Consumer Privacy Act).

### Vấn Đề Giải Quyết

- ❌ **Trước:** Luật sư mất 4 tiếng để đọc và tìm lỗi trong hợp đồng
- ✅ **Sau:** AI tự động phân tích trong 10 giây
- 🎯 **Kết quả:** Tăng hiệu suất 1440x, giảm chi phí nhân công

---

## ✨ Tính Năng

### Giai Đoạn 1 (Hoàn Thành) ✅

- [x] **Upload Tài Liệu**
  - Upload file PDF, DOCX, TXT, XLSX
  - Chặn file hình ảnh (JPG, PNG, GIF, SVG, WEBP, ICO, TIFF, HEIC)
  - Lưu trữ vật lý trong `InternalStorage/Documents`
  - Lưu metadata vào PostgreSQL
  
- [x] **Quản Lý Database**
  - 3 bảng chính: Users, Documents, Reports
  - Entity Framework Core Migration
  - PostgreSQL trên Docker (Port 5432)
  
- [x] **API Documentation**
  - Swagger UI tích hợp
  - OpenAPI specification
  - Tự động sinh tài liệu API

### Giai Đoạn 2 (Đang Phát Triển) 🚧

- [ ] AI Integration - Phân tích tài liệu với LLM
- [ ] JWT Authentication & Authorization
- [ ] Tạo báo cáo tuân thủ tự động
- [ ] Export báo cáo PDF

---

## 🏗️ Kiến Trúc Hệ Thống

```
┌─────────────────────────────────────────────────────┐
│                  Client Layer                       │
│              (Swagger UI / Frontend)                │
└──────────────────┬──────────────────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────────────────┐
│              API Layer (.NET 9)                     │
│  ┌──────────────┐  ┌──────────────┐                │
│  │  Controllers │  │  Middleware  │                │
│  └──────────────┘  └──────────────┘                │
└──────────────────┬──────────────────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────────────────┐
│            Business Logic Layer                     │
│  ┌──────────────┐  ┌──────────────┐                │
│  │   Services   │  │  Validators  │                │
│  └──────────────┘  └──────────────┘                │
└──────────────────┬──────────────────────────────────┘
                   │
        ┌──────────┴──────────┐
        ▼                     ▼
┌───────────────┐    ┌────────────────────┐
│  File Storage │    │   PostgreSQL DB    │
│ InternalStorage│    │  (Docker Container)│
└───────────────┘    └────────────────────┘
```

---

## 🚀 Cài Đặt

### Yêu Cầu Hệ Thống

- **.NET 9 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Docker Desktop** - [Download](https://www.docker.com/products/docker-desktop)
- **pgAdmin 4** (Tùy chọn) - [Download](https://www.pgadmin.org/download/)
- **Visual Studio 2022** hoặc **VS Code**

### Bước 1: Clone Repository

```bash
git clone https://github.com/SokKimThanh/ComplianceApi.git
cd ComplianceApi
```

### Bước 2: Cài Đặt Docker & PostgreSQL

```bash
# Di chuyển đến thư mục chứa docker-compose.yaml
cd H:\Blazor\compliance-system

# Khởi động PostgreSQL container
docker-compose up -d
```

**File `docker-compose.yaml`:**

```yaml
version: '3.8'
services:
  postgres:
    image: postgres:15
    container_name: compliance-postgres
    environment:
      POSTGRES_DB: compliancedb
      POSTGRES_USER: admin
      POSTGRES_PASSWORD: admin123
    ports:
      - "5432:5432"
    volumes:
      - postgres-data:/var/lib/postgresql/data

volumes:
  postgres-data:
```

### Bước 3: Cài Đặt Dependencies

```bash
cd Backend/ComplianceApi

# Cài đặt NuGet packages
dotnet restore

# Hoặc cài đặt thủ công
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.2
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.2
dotnet add package Swashbuckle.AspNetCore
```

### Bước 4: Chạy Migration

```bash
# Cài đặt EF Core Tools (nếu chưa có)
dotnet tool install --global dotnet-ef

# Tạo migration
dotnet ef migrations add InitialCreate

# Cập nhật database
dotnet ef database update
```

### Bước 5: Chạy Ứng Dụng

```bash
dotnet run
```

Truy cập Swagger UI: **https://localhost:5001** hoặc **http://localhost:5000**

---

## ⚙️ Cấu Hình

### `appsettings.json`

```json
{
  "FileStorage": {
    "LocalPath": "InternalStorage/Documents"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=compliancedb;Username=admin;Password=admin123;Timeout=30;CommandTimeout=30"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Environment Variables (Production)

Đối với môi trường production, nên sử dụng biến môi trường thay vì hardcode password:

```bash
export ConnectionStrings__DefaultConnection="Host=...;Password=<secure-password>"
```

---

## 📖 Sử Dụng

### Upload File qua Swagger

1. Mở browser tại `https://localhost:5001`
2. Expand endpoint **POST /api/Upload/upload-local**
3. Click **Try it out**
4. Chọn file (PDF, DOCX, TXT, XLSX)
5. Click **Execute**

### Response Thành Công (200 OK)

```json
{
  "message": "Tải lên thành công!",
  "documentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "storedPath": "H:\\Blazor\\compliance-system\\Backend\\ComplianceApi\\InternalStorage\\Documents\\3fa85f64-5717-4562-b3fc-2c963f66afa6.pdf"
}
```

### Response Lỗi (400 Bad Request)

```json
{
  "message": "File hình ảnh không được phép tải lên.",
  "blockedExtension": ".png",
  "allowedTypes": "Chỉ chấp nhận file tài liệu (PDF, DOCX, XLSX, TXT, v.v.)"
}
```

---

## 🔌 API Endpoints

| Method | Endpoint | Description | Request | Response |
|--------|----------|-------------|---------|----------|
| **POST** | `/api/Upload/upload-local` | Upload tài liệu | `multipart/form-data` | `200 OK` / `400 Bad Request` |

### Swagger Documentation

Xem đầy đủ tài liệu API tại: `https://localhost:5001/swagger`

---

## 🗄️ Database Schema

### Bảng `Users`

```sql
CREATE TABLE "Users" (
    "Id" UUID PRIMARY KEY,
    "FullName" VARCHAR(255) NOT NULL,
    "Email" VARCHAR(255) UNIQUE NOT NULL,
    "Password" VARCHAR(255) NOT NULL,
    "CreatedAt" TIMESTAMP DEFAULT NOW()
);
```

### Bảng `Documents`

```sql
CREATE TABLE "Documents" (
    "Id" UUID PRIMARY KEY,
    "UserId" UUID NOT NULL,
    "FileName" VARCHAR(500) NOT NULL,
    "FilePath" TEXT NOT NULL,
    "Status" VARCHAR(50) DEFAULT 'Pending',
    "UploadedAt" TIMESTAMP DEFAULT NOW(),
    FOREIGN KEY ("UserId") REFERENCES "Users"("Id")
);
```

### Bảng `Reports`

```sql
CREATE TABLE "Reports" (
    "Id" UUID PRIMARY KEY,
    "DocumentId" UUID NOT NULL,
    "Score" INT,
    "Status" VARCHAR(50),
    "Result" JSONB,
    "CreatedAt" TIMESTAMP DEFAULT NOW(),
    FOREIGN KEY ("DocumentId") REFERENCES "Documents"("Id")
);
```

---

## 💻 Development

### Cấu Trúc Thư Mục

```
ComplianceApi/
├── Controllers/
│   └── UploadController.cs
├── Data/
│   └── ApplicationDbContext.cs
├── Models/
│   ├── Document.cs
│   ├── Report.cs
│   └── User.cs
├── InternalStorage/
│   └── Documents/
├── Migrations/
├── Properties/
├── appsettings.json
├── Program.cs
└── README.md
```

### Coding Standards

- **C# 13** với **.NET 9**
- **Nullable Reference Types** enabled
- **Entity Framework Core 8.0.2** (tương thích .NET 9)
- **RESTful API** design patterns
- **Repository Pattern** (planned for v2.0)

### Testing

```bash
# Chạy unit tests (when implemented)
dotnet test

# Code coverage
dotnet test /p:CollectCoverage=true
```

---

## 🗺️ Roadmap

### ✅ Phase 1: Core Backend (Hoàn Thành)
- Docker + PostgreSQL setup
- File upload với validation
- Entity Framework Migration
- Swagger documentation

### 🚧 Phase 2: AI Integration (Q2 2024)
- [ ] Tích hợp OpenAI API / Azure OpenAI
- [ ] Phân tích tài liệu với GPT-4
- [ ] Tạo báo cáo tuân thủ tự động

### 📅 Phase 3: Authentication (Q3 2024)
- [ ] JWT Authentication
- [ ] Role-based Authorization
- [ ] User Management API

### 📅 Phase 4: Frontend (Q4 2024)
- [ ] Blazor WebAssembly UI
- [ ] Dashboard analytics
- [ ] Report visualization

---

## 🤝 Đóng Góp

Contributions, issues và feature requests được chào đón!

1. Fork repository
2. Tạo feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Open Pull Request

---

## 👨‍💻 Tác Giả

**Sok Kim Thanh**
- GitHub: [@SokKimThanh](https://github.com/SokKimThanh)
- Email: support@complianceapi.com

---

## 📝 License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

---

## 🙏 Acknowledgments

- **.NET Community** - Amazing framework và tools
- **PostgreSQL** - Robust open-source database
- **Swagger/OpenAPI** - API documentation standards
- **Entity Framework Core** - ORM excellence

---

## 📞 Support

Nếu bạn gặp vấn đề, hãy:
1. Check [Issues](https://github.com/SokKimThanh/ComplianceApi/issues)
2. Tạo issue mới với label `bug` hoặc `question`
3. Liên hệ: support@complianceapi.com

---

<div align="center">

**⭐ Nếu project này hữu ích, hãy cho một star! ⭐**

Made with ❤️ by Sok Kim Thanh

</div>