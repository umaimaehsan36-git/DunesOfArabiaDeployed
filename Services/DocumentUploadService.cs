using Microsoft.EntityFrameworkCore;
using DunesOfArabia.Data;
using DunesOfArabia.Models;

namespace DunesOfArabia.Services
{
    public interface IDocumentUploadService
    {
        Task<UploadedDocumentResult> UploadAsync(string userId, UploadDocumentRequest request);
        Task<List<UploadedDocumentResult>> GetUserDocumentsAsync(string userId);
        Task SetCategoryAsync(int documentId, string category);
        Task DeleteAsync(int documentId, string userId);
    }

    public record UploadDocumentRequest(
        string FileName,
        string FileType,
        byte[] FileBytes,
        string ContentType
    );

    public class UploadedDocumentResult
    {
        public int Id { get; set; }
        public string FileName { get; set; } = "";
        public string FileType { get; set; } = "";
        /// <summary>Booking gate category: Passport | National ID | Selfie | Visa | Ticket | Other</summary>
        public string Category { get; set; } = "Other";
        public long FileSizeBytes { get; set; }
        public string? FileUrl { get; set; }
        public DateTime UploadedOn { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// DB-backed implementation.
    /// Files are saved to wwwroot/uploads/{userId}/.
    /// Replace the file-write block with Azure Blob / S3 calls for production.
    /// </summary>
    public class DocumentUploadService : IDocumentUploadService
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public DocumentUploadService(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public async Task<UploadedDocumentResult> UploadAsync(string userId, UploadDocumentRequest req)
        {
            var folder = Path.Combine(_env.WebRootPath, "uploads", userId);
            Directory.CreateDirectory(folder);
            var safeFile = $"{Guid.NewGuid()}_{Path.GetFileName(req.FileName)}";
            var filePath = Path.Combine(folder, safeFile);
            await File.WriteAllBytesAsync(filePath, req.FileBytes);
            var fileUrl = $"/uploads/{userId}/{safeFile}";

            var doc = new UserDocument
            {
                UserId = userId,
                FileName = req.FileName,
                FileType = req.FileType,
                Category = "Other",
                FileSizeBytes = req.FileBytes.LongLength,
                FileUrl = fileUrl,
                UploadedOn = DateTime.UtcNow
            };
            _db.UserDocuments.Add(doc);
            await _db.SaveChangesAsync();
            return MapToResult(doc);
        }

        public async Task<List<UploadedDocumentResult>> GetUserDocumentsAsync(string userId)
        {
            var docs = await _db.UserDocuments
                .AsNoTracking()
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.UploadedOn)
                .ToListAsync();
            return docs.Select(MapToResult).ToList();
        }

        public async Task SetCategoryAsync(int documentId, string category)
        {
            var doc = await _db.UserDocuments.FindAsync(documentId);
            if (doc is null) return;
            doc.Category = category;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int documentId, string userId)
        {
            var doc = await _db.UserDocuments
                .FirstOrDefaultAsync(d => d.Id == documentId && d.UserId == userId);
            if (doc is null) return;

            if (!string.IsNullOrEmpty(doc.FileUrl))
            {
                var physicalPath = Path.Combine(
                    _env.WebRootPath,
                    doc.FileUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(physicalPath))
                    File.Delete(physicalPath);
            }

            _db.UserDocuments.Remove(doc);
            await _db.SaveChangesAsync();
        }

        private static UploadedDocumentResult MapToResult(UserDocument d) => new()
        {
            Id = d.Id,
            FileName = d.FileName,
            FileType = d.FileType,
            Category = d.Category,
            FileSizeBytes = d.FileSizeBytes,
            FileUrl = d.FileUrl,
            UploadedOn = d.UploadedOn
        };
    }
}