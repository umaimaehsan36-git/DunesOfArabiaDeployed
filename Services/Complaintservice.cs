using Microsoft.EntityFrameworkCore;
using DunesOfArabia.Data;
using DunesOfArabia.Models;

namespace DunesOfArabia.Services
{
    public interface IComplaintService
    {
        Task<List<Complaint>> GetUserComplaintsAsync(string userId);
        Task<List<Complaint>> GetAllComplaintsAsync();
        Task<Complaint> SubmitComplaintAsync(Complaint complaint);
        Task<bool> RespondAsync(int id, string adminResponse);
        Task<bool> ResolveAsync(int id);
        Task<bool> UpdateStatusAsync(int id, string newStatus);
    }

    public class ComplaintService : IComplaintService
    {
        private readonly AppDbContext _db;
        public ComplaintService(AppDbContext db) { _db = db; }

        public async Task<List<Complaint>> GetUserComplaintsAsync(string userId)
            => await _db.Complaints
                .AsNoTracking()
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

        public async Task<List<Complaint>> GetAllComplaintsAsync()
            => await _db.Complaints
                .AsNoTracking()
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

        public async Task<Complaint> SubmitComplaintAsync(Complaint complaint)
        {
            complaint.Status = "Open";
            complaint.CreatedAt = DateTime.UtcNow;
            _db.Complaints.Add(complaint);
            await _db.SaveChangesAsync();
            return complaint;
        }

        public async Task<bool> RespondAsync(int id, string adminResponse)
        {
            var c = await _db.Complaints.FindAsync(id);
            if (c is null) return false;
            c.AdminResponse = adminResponse;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ResolveAsync(int id)
        {
            var c = await _db.Complaints.FindAsync(id);
            if (c is null) return false;
            c.Status = "Resolved";
            c.ResolvedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateStatusAsync(int id, string newStatus)
        {
            var c = await _db.Complaints.FindAsync(id);
            if (c is null) return false;
            c.Status = newStatus;
            if (newStatus == "Resolved") c.ResolvedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}