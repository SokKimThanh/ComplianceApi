-- 1. Tạo bảng Users
CREATE TABLE IF NOT EXISTS "Users" (
    "Id" UUID PRIMARY KEY,
    "FullName" TEXT NOT NULL,
    "Email" TEXT UNIQUE NOT NULL,
    "PasswordHash" TEXT NOT NULL,
    "Role" TEXT DEFAULT 'User',
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 2. Tạo bảng Documents
CREATE TABLE IF NOT EXISTS "Documents" (
    "Id" UUID PRIMARY KEY,
    "UserId" UUID REFERENCES "Users"("Id"),
    "FileName" TEXT NOT NULL,
    "FilePath" TEXT NOT NULL,
    "Status" TEXT NOT NULL, -- Pending, Processing, Completed, Failed
    "UploadedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 3. Tạo bảng Reports
CREATE TABLE IF NOT EXISTS "Reports" (
    "Id" UUID PRIMARY KEY,
    "DocumentId" UUID REFERENCES "Documents"("Id") ON DELETE CASCADE,
    "ComplianceScore" INT,
    "Status" TEXT, -- Compliant, Warning, Non-Compliant
    "AIResultJson" JSONB,
    "GeneratedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 4. Chèn dữ liệu mẫu (Seed Data)
INSERT INTO "Users" ("Id", "FullName", "Email", "PasswordHash", "Role")
VALUES ('550e8400-e29b-41d4-a716-446655440000', 'Sok Kim Thanh', 'thanh.sok@example.com', 'hashed_password', 'Admin');

INSERT INTO "Documents" ("Id", "UserId", "FileName", "FilePath", "Status")
VALUES ('d290f1ee-6c54-4b01-90e6-d701748f0851', '550e8400-e29b-41d4-a716-446655440000', 'Contract_GDPR_Check.pdf', '/uploads/contract1.pdf', 'Completed');

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

INSERT INTO "Reports" ("Id", "DocumentId", "ComplianceScore", "Status", "AIResultJson")
VALUES (
    uuid_generate_v4(), 
    'd290f1ee-6c54-4b01-90e6-d701748f0851', 
    85, 
    'Compliant', 
    '{"issues": [{"clause": "Article 5", "problem": "Clear consent", "reference": "GDPR Art 6"}]}'
);

