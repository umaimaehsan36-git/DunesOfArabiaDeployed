using Microsoft.EntityFrameworkCore;
using DunesOfArabia.Data;
using DunesOfArabia.Models;

namespace DunesOfArabia.Services
{
    public interface IActivityService
    {
        Task<List<Activity>> GetAllAsync();
        Task<Activity?> GetByIdAsync(int id);
        Task<List<Activity>> GetByDestinationAsync(int destinationId);
        Task<List<Activity>> GetByDestinationIdAsync(int destinationId);
        Task<Activity> CreateAsync(CreateActivityDto dto);
        Task<Activity?> UpdateAsync(int id, UpdateActivityDto dto);
        Task DeleteAsync(int id);
    }

    public class ActivityService : IActivityService
    {
        private readonly AppDbContext _db;
        public ActivityService(AppDbContext db) { _db = db; }

        public async Task<List<Activity>> GetAllAsync()
            => await _db.Activities
                .AsNoTracking()
                .OrderBy(a => a.Category)
                .ThenBy(a => a.Name)
                .ToListAsync();

        public async Task<Activity?> GetByIdAsync(int id)
            => await _db.Activities
                .AsNoTracking()
                .Include(a => a.Destination)
                .FirstOrDefaultAsync(a => a.Id == id);

        public async Task<List<Activity>> GetByDestinationAsync(int destinationId)
            => await _db.Activities
                .AsNoTracking()
                .Where(a => a.DestinationId == destinationId)
                .OrderBy(a => a.Name)
                .ToListAsync();

        public Task<List<Activity>> GetByDestinationIdAsync(int destinationId)
            => GetByDestinationAsync(destinationId);

        public async Task<Activity> CreateAsync(CreateActivityDto dto)
        {
            var activity = new Activity
            {
                DestinationId = dto.DestinationId,
                Name = dto.Name,
                Description = dto.Description,
                DurationHours = dto.DurationHours,
                PriceSAR = dto.PriceSAR,
                Category = dto.Category,
                ImageUrl = dto.ImageUrl,
                DifficultyLevel = dto.DifficultyLevel ?? "Easy",
                OperatorName = dto.OperatorName ?? "",
                OperatorEmail = dto.OperatorEmail ?? "",
                OperatorPhone = dto.OperatorPhone ?? "",
                CancellationPolicy = dto.CancellationPolicy ?? ""
            };
            _db.Activities.Add(activity);
            await _db.SaveChangesAsync();
            return activity;
        }

        public async Task<Activity?> UpdateAsync(int id, UpdateActivityDto dto)
        {
            var a = await _db.Activities.FindAsync(id);
            if (a is null) return null;

            if (dto.Name is not null) a.Name = dto.Name;
            if (dto.Description is not null) a.Description = dto.Description;
            if (dto.DurationHours is not null) a.DurationHours = dto.DurationHours.Value;
            if (dto.PriceSAR is not null) a.PriceSAR = dto.PriceSAR.Value;
            if (dto.Category is not null) a.Category = dto.Category;
            if (dto.ImageUrl is not null) a.ImageUrl = dto.ImageUrl;
            if (dto.DifficultyLevel is not null) a.DifficultyLevel = dto.DifficultyLevel;
            if (dto.OperatorName is not null) a.OperatorName = dto.OperatorName;
            if (dto.OperatorEmail is not null) a.OperatorEmail = dto.OperatorEmail;
            if (dto.OperatorPhone is not null) a.OperatorPhone = dto.OperatorPhone;
            if (dto.CancellationPolicy is not null) a.CancellationPolicy = dto.CancellationPolicy;

            await _db.SaveChangesAsync();
            return a;
        }

        public async Task DeleteAsync(int id)
        {
            var a = await _db.Activities.FindAsync(id);
            if (a is null) return;
            _db.Activities.Remove(a);
            await _db.SaveChangesAsync();
        }
    }

    public record CreateActivityDto(
        int DestinationId,
        string Name,
        string Description,
        decimal DurationHours,
        decimal PriceSAR,
        string Category,
        string ImageUrl,
        string? DifficultyLevel = null,
        string? OperatorName = null,
        string? OperatorEmail = null,
        string? OperatorPhone = null,
        string? CancellationPolicy = null
    );

    public record UpdateActivityDto(
        string? Name,
        string? Description,
        decimal? DurationHours,
        decimal? PriceSAR,
        string? Category,
        string? ImageUrl,
        string? DifficultyLevel = null,
        string? OperatorName = null,
        string? OperatorEmail = null,
        string? OperatorPhone = null,
        string? CancellationPolicy = null
    );
}