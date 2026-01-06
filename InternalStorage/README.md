# ?? Internal Storage Directory

## M?c ?ích

Th? m?c này ch?a các file ???c upload b?i users qua API endpoint `/api/Upload/upload`.

## C?u Trúc

```
InternalStorage/
??? Documents/
    ??? .gitkeep                    # Gi? c?u trúc th? m?c trong Git
    ??? {guid}.pdf                  # File uploads (ignored by Git)
    ??? {guid}.docx                 # File uploads (ignored by Git)
    ??? ...
```

## Git Configuration

### ? ???c track:
- `.gitkeep` - Gi? c?u trúc th? m?c
- `README.md` - Documentation

### ? KHÔNG ???c track:
- T?t c? file uploads t? users
- File formats: `.pdf`, `.docx`, `.xlsx`, `.txt`, etc.

## Lý Do

1. **Security:** Tránh commit sensitive documents vào Git
2. **Repository Size:** Gi? repo nh?, không ch?a binary files
3. **Privacy:** User-uploaded content không ???c public
4. **Best Practice:** File storage nên ? S3/Azure Blob, không ph?i Git

## Production Setup

Trong production, nên:

1. **Cloud Storage:** Migrate sang AWS S3, Azure Blob Storage
2. **CDN:** S? d?ng CloudFront/Azure CDN cho delivery
3. **Backup:** Automated backup strategy
4. **Retention Policy:** T? ??ng xóa file c?

## Local Development

### T?o th? m?c (n?u ch?a có):

```bash
mkdir -p InternalStorage/Documents
```

### Ki?m tra c?u hình Git:

```bash
# File .gitkeep ph?i ???c track
git status InternalStorage/Documents/.gitkeep

# Uploaded files KHÔNG ???c track
git status InternalStorage/Documents/*.pdf
```

### Test upload:

```bash
# Upload file qua Swagger UI
POST /api/Upload/upload

# Ki?m tra file ?ã l?u
ls -la InternalStorage/Documents/
```

## Cleanup

### Xóa t?t c? uploaded files (local):

```bash
# PowerShell
Remove-Item -Path "InternalStorage/Documents/*" -Exclude ".gitkeep"

# Bash/Linux
find InternalStorage/Documents/ -type f ! -name '.gitkeep' -delete
```

### Scheduled cleanup (Production):

```csharp
// Background service t? ??ng xóa file c? > 30 ngày
var oldFiles = Directory.GetFiles(storagePath)
    .Where(f => File.GetCreationTime(f) < DateTime.Now.AddDays(-30));

foreach (var file in oldFiles)
{
    File.Delete(file);
}
```

## Security Notes

?? **QUAN TR?NG:**

1. **Không commit** file uploads vào Git
2. **Validate** file extensions tr??c khi l?u
3. **Scan** virus/malware (production)
4. **Encrypt** sensitive documents
5. **Access Control** - Ch? authorized users ???c download

## Monitoring

Track metrics:

- Total storage used
- Number of files
- Upload rate
- Failed uploads
- File types distribution

---

**Tác gi?:** Compliance API Team  
**Version:** 1.0  
**Last Updated:** 2024-01-15
