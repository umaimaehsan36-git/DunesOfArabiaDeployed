using Microsoft.EntityFrameworkCore;
using DunesOfArabia.Data;
using DunesOfArabia.Models;

namespace DunesOfArabia.Services
{
    public interface IDestinationService
    {
        Task<List<Destination>> GetAllAsync();
        Task<Destination?> GetByIdAsync(int id);

        // FIX: Batch load by multiple IDs — eliminates N+1 query pattern in
        // MyBookings.razor, MyItineraries.razor, and Profile.razor.
        // One SQL WHERE id IN (...) instead of N individual SELECT queries.
        Task<List<Destination>> GetByIdsAsync(IEnumerable<int> ids);

        Task<List<Destination>> SearchAsync(string keyword);
        Task<List<Destination>> FilterAsync(string? category, string? province, decimal? maxCost);
        Task<List<Destination>> GetByProvinceAsync(string province);
        Task<List<Destination>> GetByCategoryAsync(string category);
        Task<Destination> CreateAsync(CreateDestinationDto dto);
        Task<Destination?> UpdateAsync(int id, UpdateDestinationDto dto);
        Task DeleteAsync(int id);
    }

    public class DestinationService : IDestinationService
    {
        private readonly AppDbContext _db;
        public DestinationService(AppDbContext db) { _db = db; }

        public async Task<List<Destination>> GetAllAsync()
            => await _db.Destinations
                .AsNoTracking()
                .OrderBy(d => d.Name)
                .ToListAsync();

        public async Task<Destination?> GetByIdAsync(int id)
            => await _db.Destinations
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);

        // FIX: New — batch fetch replaces the N+1 per-booking/per-favorite loops
        // in MyBookings, MyItineraries, and Profile razor components.
        public async Task<List<Destination>> GetByIdsAsync(IEnumerable<int> ids)
        {
            var idList = ids.ToList();
            if (!idList.Any()) return new List<Destination>();

            return await _db.Destinations
                .AsNoTracking()
                .Where(d => idList.Contains(d.Id))
                .ToListAsync();
        }

        public async Task<List<Destination>> SearchAsync(string keyword)
        {
            var lower = keyword.ToLower();
            return await _db.Destinations
                .AsNoTracking()
                .Where(d => d.Name.ToLower().Contains(lower)
                         || d.Description.ToLower().Contains(lower)
                         || d.Province.ToLower().Contains(lower))
                .ToListAsync();
        }

        public async Task<List<Destination>> FilterAsync(string? category, string? province, decimal? maxCost)
        {
            var query = _db.Destinations.AsNoTracking().AsQueryable();
            if (!string.IsNullOrEmpty(category)) query = query.Where(d => d.Category == category);
            if (!string.IsNullOrEmpty(province)) query = query.Where(d => d.Province == province);
            if (maxCost.HasValue) query = query.Where(d => d.Cost <= maxCost.Value);
            return await query.ToListAsync();
        }

        public async Task<List<Destination>> GetByProvinceAsync(string province)
            => await _db.Destinations
                .AsNoTracking()
                .Where(d => d.Province == province)
                .ToListAsync();

        public async Task<List<Destination>> GetByCategoryAsync(string category)
            => await _db.Destinations
                .AsNoTracking()
                .Where(d => d.Category == category)
                .ToListAsync();

        public async Task<Destination> CreateAsync(CreateDestinationDto dto)
        {
            var destination = new Destination
            {
                Name = dto.Name,
                Province = dto.Province,
                Category = dto.Category,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Cost = dto.Cost,
                Climate = dto.Climate,
                VisaInfo = dto.VisaInfo
            };
            _db.Destinations.Add(destination);
            await _db.SaveChangesAsync();
            return destination;
        }

        public async Task<Destination?> UpdateAsync(int id, UpdateDestinationDto dto)
        {
            var existing = await _db.Destinations.FindAsync(id);
            if (existing is null) return null;

            if (dto.Name is not null) existing.Name = dto.Name;
            if (dto.Province is not null) existing.Province = dto.Province;
            if (dto.Category is not null) existing.Category = dto.Category;
            if (dto.Description is not null) existing.Description = dto.Description;
            if (dto.ImageUrl is not null) existing.ImageUrl = dto.ImageUrl;
            if (dto.Latitude is not null) existing.Latitude = dto.Latitude.Value;
            if (dto.Longitude is not null) existing.Longitude = dto.Longitude.Value;
            if (dto.Cost is not null) existing.Cost = dto.Cost.Value;
            if (dto.Climate is not null) existing.Climate = dto.Climate;
            if (dto.VisaInfo is not null) existing.VisaInfo = dto.VisaInfo;

            await _db.SaveChangesAsync();
            return existing;
        }

        public async Task DeleteAsync(int id)
        {
            var destination = await _db.Destinations.FindAsync(id);
            if (destination is null) return;
            _db.Destinations.Remove(destination);
            await _db.SaveChangesAsync();
        }
    }

    public record CreateDestinationDto(
        string Name,
        string Province,
        string Category,
        string Description,
        string ImageUrl,
        double Latitude,
        double Longitude,
        decimal Cost,
        string Climate,
        string VisaInfo
    );

    public record UpdateDestinationDto(
        string? Name,
        string? Province,
        string? Category,
        string? Description,
        string? ImageUrl,
        double? Latitude,
        double? Longitude,
        decimal? Cost,
        string? Climate,
        string? VisaInfo
    );
}