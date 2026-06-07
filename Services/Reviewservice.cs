using Microsoft.EntityFrameworkCore;
using DunesOfArabia.Data;
using DunesOfArabia.Models;

namespace DunesOfArabia.Services
{
    public interface IReviewService
    {
        // ── Original methods ──────────────────────────────────
        Task<List<Review>> GetByDestinationAsync(int destinationId);
        Task<List<Review>> GetByUserAsync(string userId);
        Task<Review?> GetByIdAsync(int id);
        Task<Review> CreateAsync(string userId, CreateReviewDto dto);
        Task<bool> DeleteAsync(int id, string userId);
        Task<double> GetAverageRatingAsync(int destinationId);

        // ── Added: used by Razor components ───────────────────
        Task<List<Review>> GetAllAsync();
        Task<List<Review>> GetByDestinationIdAsync(int destinationId);
        Task<List<Review>> GetByActivityIdAsync(int activityId);
    }

    public class ReviewService : IReviewService
    {
        private readonly AppDbContext _db;
        public ReviewService(AppDbContext db) { _db = db; }

        // ── Include User so Review.UserName resolves correctly ─
        private IQueryable<Review> ReviewsWithUser()
            => _db.Reviews.Include(r => r.User);

        public async Task<List<Review>> GetAllAsync()
            => await ReviewsWithUser()
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

        public async Task<List<Review>> GetByDestinationAsync(int destinationId)
            => await ReviewsWithUser()
                .Where(r => r.DestinationId == destinationId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

        // Alias — Razor components call this name
        public Task<List<Review>> GetByDestinationIdAsync(int destinationId)
            => GetByDestinationAsync(destinationId);

        public async Task<List<Review>> GetByActivityIdAsync(int activityId)
            => await ReviewsWithUser()
                .Where(r => r.ActivityId == activityId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

        public async Task<List<Review>> GetByUserAsync(string userId)
            => await ReviewsWithUser()
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

        public async Task<Review?> GetByIdAsync(int id)
            => await ReviewsWithUser().FirstOrDefaultAsync(r => r.Id == id);

        public async Task<Review> CreateAsync(string userId, CreateReviewDto dto)
        {
            var review = new Review
            {
                UserId = userId,
                DestinationId = dto.DestinationId,
                ActivityId = dto.ActivityId,
                StarRating = dto.StarRating,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };
            _db.Reviews.Add(review);
            await _db.SaveChangesAsync();

            // Update destination average rating
            var avg = await GetAverageRatingAsync(dto.DestinationId);
            var dest = await _db.Destinations.FindAsync(dto.DestinationId);
            if (dest is not null) { dest.Rating = avg; await _db.SaveChangesAsync(); }

            return review;
        }

        public async Task<bool> DeleteAsync(int id, string userId)
        {
            var review = await _db.Reviews
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
            if (review is null) return false;
            _db.Reviews.Remove(review);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<double> GetAverageRatingAsync(int destinationId)
        {
            var ratings = await _db.Reviews
                .Where(r => r.DestinationId == destinationId)
                .Select(r => r.StarRating)
                .ToListAsync();
            return ratings.Count == 0 ? 0 : ratings.Average();
        }
    }

    public record CreateReviewDto(
        int DestinationId,
        int StarRating,
        string Comment,
        int? ActivityId = null
    );
}