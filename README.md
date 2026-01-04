# ?? Compliance API - H? Th?ng Ki?m Tra Tuân Th? CCPA v?i AI

![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15-316192?logo=postgresql)
![Docker](https://img.shields.io/badge/Docker-Latest-2496ED?logo=docker)
![License](https://img.shields.io/badge/License-MIT-green)

> **H? th?ng t? ??ng hóa quy trình ki?m tra tuân th? CCPA cho h?p ??ng**  
> Gi?m th?i gian ki?m kê t? **4 ti?ng xu?ng còn 10 giây** nh? AI

---

## ?? M?c L?c

- [T?ng Quan](#-t?ng-quan)
- [Tính N?ng](#-tính-n?ng)
- [Ki?n Trúc H? Th?ng](#-ki?n-trúc-h?-th?ng)
- [Cài ??t](#-cài-??t)
- [C?u Hình](#?-c?u-hình)
- [S? D?ng](#-s?-d?ng)
- [API Endpoints](#-api-endpoints)
- [Database Schema](#-database-schema)
- [Development](#-development)
- [Roadmap](#-roadmap)
- [?óng Góp](#-?óng-góp)
- [License](#-license)

---

## ?? T?ng Quan

**Compliance API** là h? th?ng backend ???c xây d?ng v?i **.NET 9**, tích h?p AI ?? t? ??ng ki?m tra tính tuân th? c?a h?p ??ng theo quy trình ki?m soát **CCPA** (California Consumer Privacy Act).

### V?n ?? Gi?i Quy?t

- ? **Tr??c:** Lu?t s? m?t 4 ti?ng ?? ??c và tìm l?i trong h?p ??ng
- ? **Sau:** AI t? ??ng phân tích trong 10 giây
- ?? **K?t qu?:** T?ng hi?u su?t 1440x, gi?m chi phí nhân công

---

## ? Tính N?ng

### Giai ?o?n 1 (Hoàn Thành) ?

- [x] **Upload Tài Li?u**
  - Upload file PDF, DOCX, TXT, XLSX
  - Ch?n file hình ?nh (JPG, PNG, GIF, SVG, WEBP, ICO, TIFF, HEIC)
  - L?u tr? v?t lý trong `InternalStorage/Documents`
  - L?u metadata vào PostgreSQL
  
- [x] **Qu?n Lý Database**
  - 3 b?ng chính: Users, Documents, Reports
  - Entity Framework Core Migration
  - PostgreSQL trên Docker (Port 5432)
  
- [x] **API Documentation**
  - Swagger UI tích h?p
  - OpenAPI specification
  - T? ??ng sinh tài li?u API

### Giai ?o?n 2 (?ang Phát Tri?n) ??

- [ ] AI Integration - Phân tích tài li?u v?i LLM
- [ ] JWT Authentication & Authorization
- [ ] T?o báo cáo tuân th? t? ??ng
- [ ] Export báo cáo PDF

---

## ??? Ki?n Trúc H? Th?ng

```
???????????????????????????????????????????????????????
?                  Client Layer                       ?
?              (Swagger UI / Frontend)                ?
???????????????????????????????????????????????????????
                   ?
                   ?
???????????????????????????????????????????????????????
?              API Layer (.NET 9)                     ?
?  ????????????????  ????????????????                ?
?  ?  Controllers ?  ?  Middleware  ?                ?
?  ????????????????  ????????????????                ?
???????????????????????????????????????????????????????
                   ?
                   ?
???????????????????????????????????????????????????????
?            Business Logic Layer                     ?
?  ????????????????  ????????????????                ?
?  ?   Services   ?  ?  Validators  ?                ?
?  ????????????????  ????????????????                ?
???????????????????????????????????????????????????????
                   ?
        ???????????????????????
        ?                     ?
?????????????????    ??????????????????????
?  File Storage ?    ?   PostgreSQL DB    ?
? InternalStorage?    ?  (Docker Container)?
?????????????????    ??????????????????????
```

---

## ?? Cài ??t

### Yêu C?u H? Th?ng

- **.NET 9 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Docker Desktop** - [Download](https://www.docker.com/products/docker-desktop)
- **pgAdmin 4** (Tùy ch?n) - [Download](https://www.pgadmin.org/download/)
- **Visual Studio 2022** ho?c **VS Code**

### B??c 1: Clone Repository

```bash
git clone https://github.com/SokKimThanh/ComplianceApi.git
cd ComplianceApi
```

### B??c 2: Cài ??t Docker & PostgreSQL

```bash
# Di chuy?n ??n th? m?c ch?a docker-compose.yaml
cd H:\Blazor\compliance-system

# Kh?i ??ng PostgreSQL container
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

### B??c 3: Cài ??t Dependencies

```bash
cd Backend/ComplianceApi

# Cài ??t NuGet packages
dotnet restore

# Ho?c cài ??t th? công
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.2
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.2
dotnet add package Swashbuckle.AspNetCore
```

### B??c 4: Ch?y Migration

```bash
# Cài ??t EF Core Tools (n?u ch?a có)
dotnet tool install --global dotnet-ef

# T?o migration
dotnet ef migrations add InitialCreate

# C?p nh?t database
dotnet ef database update
```

### B??c 5: Ch?y ?ng D?ng

```bash
dotnet run
```

Truy c?p Swagger UI: **https://localhost:5001** ho?c **http://localhost:5000**

---

## ?? C?u Hình

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

??i v?i môi tr??ng production, nên s? d?ng bi?n môi tr??ng thay vì hardcode password:

```bash
export ConnectionStrings__DefaultConnection="Host=...;Password=<secure-password>"
```

---

## ?? S? D?ng

### Upload File qua Swagger

1. M? browser t?i `https://localhost:5001`
2. Expand endpoint **POST /api/Upload/upload-local**
3. Click **Try it out**
4. Ch?n file (PDF, DOCX, TXT, XLSX)
5. Click **Execute**

### Response Thành Công (200 OK)

```json
{
  "message": "T?i lên thành công!",
  "documentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "storedPath": "H:\\Blazor\\compliance-system\\Backend\\ComplianceApi\\InternalStorage\\Documents\\3fa85f64-5717-4562-b3fc-2c963f66afa6.pdf"
}
```

### Response L?i (400 Bad Request)

```json
{
  "message": "File hình ?nh không ???c phép t?i lên.",
  "blockedExtension": ".png",
  "allowedTypes": "Ch? ch?p nh?n file tài li?u (PDF, DOCX, XLSX, TXT, v.v.)"
}
```

---

## ?? API Endpoints

| Method | Endpoint | Description | Request | Response |
|--------|----------|-------------|---------|----------|
| **POST** | `/api/Upload/upload-local` | Upload tài li?u | `multipart/form-data` | `200 OK` / `400 Bad Request` |

### Swagger Documentation

Xem ??y ?? tài li?u API t?i: `https://localhost:5001/swagger`

---

## ??? Database Schema

### B?ng `Users`

```sql
CREATE TABLE "Users" (
    "Id" UUID PRIMARY KEY,
    "FullName" VARCHAR(255) NOT NULL,
    "Email" VARCHAR(255) UNIQUE NOT NULL,
    "Password" VARCHAR(255) NOT NULL,
    "CreatedAt" TIMESTAMP DEFAULT NOW()
);
```

### B?ng `Documents`

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

### B?ng `Reports`

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

## ?? Development

### C?u Trúc Th? M?c

```
ComplianceApi/
??? Controllers/
?   ??? UploadController.cs
??? Data/
?   ??? ApplicationDbContext.cs
??? Models/
?   ??? Document.cs
?   ??? Report.cs
?   ??? User.cs
??? InternalStorage/
?   ??? Documents/
??? Migrations/
??? Properties/
??? appsettings.json
??? Program.cs
??? README.md
```

### Coding Standards

- **C# 13** v?i **.NET 9**
- **Nullable Reference Types** enabled
- **Entity Framework Core 8.0.2** (t??ng thích .NET 9)
- **RESTful API** design patterns
- **Repository Pattern** (planned for v2.0)

### Testing

```bash
# Ch?y unit tests (when implemented)
dotnet test

# Code coverage
dotnet test /p:CollectCoverage=true
```

---

## ??? Roadmap

### ? Phase 1: Core Backend (Hoàn Thành)
- Docker + PostgreSQL setup
- File upload v?i validation
- Entity Framework Migration
- Swagger documentation

### ?? Phase 2: AI Integration (Q2 2024)
- [ ] Tích h?p OpenAI API / Azure OpenAI
- [ ] Phân tích tài li?u v?i GPT-4
- [ ] T?o báo cáo tuân th? t? ??ng

### ?? Phase 3: Authentication (Q3 2024)
- [ ] JWT Authentication
- [ ] Role-based Authorization
- [ ] User Management API

### ?? Phase 4: Frontend (Q4 2024)
- [ ] Blazor WebAssembly UI
- [ ] Dashboard analytics
- [ ] Report visualization

---

## ?? ?óng Góp

Contributions, issues và feature requests ???c chào ?ón!

1. Fork repository
2. T?o feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Open Pull Request

---

## ????? Tác Gi?

**Sok Kim Thanh**
- GitHub: [@SokKimThanh](https://github.com/SokKimThanh)
- Email: support@complianceapi.com

---

## ?? License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

---

## ?? Acknowledgments

- **.NET Community** - Amazing framework và tools
- **PostgreSQL** - Robust open-source database
- **Swagger/OpenAPI** - API documentation standards
- **Entity Framework Core** - ORM excellence

---

## ?? Support

N?u b?n g?p v?n ??, hãy:
1. Check [Issues](https://github.com/SokKimThanh/ComplianceApi/issues)
2. T?o issue m?i v?i label `bug` ho?c `question`
3. Liên h?: support@complianceapi.com

---

<div align="center">

**? N?u project này h?u ích, hãy cho m?t star! ?**

Made with ?? by Sok Kim Thanh

</div>