using Microsoft.EntityFrameworkCore;
using DunesOfArabia.Data;
using DunesOfArabia.Models;

namespace DunesOfArabia.Services
{
    public interface IItineraryService
    {
        Task<List<Itinerary>> GetAllAsync();
        Task<List<Itinerary>> GetByUserIdAsync(string userId);
        Task<List<Itinerary>> GetUserItinerariesAsync(string userId);
        Task<Itinerary?> GetByIdAsync(int id);
        Task<Itinerary> CreateAsync(string userId, CreateItineraryDto dto);
        Task<Itinerary> CreateAsync(Itinerary itinerary);
        Task<Itinerary> SaveAsync(Itinerary itinerary);
        Task<Itinerary?> UpdateAsync(int id, UpdateItineraryDto dto);
        Task<Itinerary?> UpdateAsync(int id, string userId, string title, DateTime startDate, DateTime endDate);
        Task DeleteAsync(int id);
        Task<bool> DeleteAsync(int id, string userId);
        Task<bool> AddActivityAsync(int itineraryId, DailyActivity activity);
        Task<bool> AddPackingItemAsync(int itineraryId, PackingItem item);
        Task<bool> TogglePackingItemAsync(int itemId);
        Task<bool> RemovePackingItemAsync(int itemId);
    }

    public class ItineraryService : IItineraryService
    {
        private readonly AppDbContext _db;
        public ItineraryService(AppDbContext db) { _db = db; }

        public async Task<List<Itinerary>> GetAllAsync()
            => await _db.Itineraries
                .Include(i => i.Activities)
                .Include(i => i.PackingItems)
                .AsNoTracking()
                .ToListAsync();

        public Task<List<Itinerary>> GetByUserIdAsync(string userId)
            => GetUserItinerariesAsync(userId);

        public async Task<List<Itinerary>> GetUserItinerariesAsync(string userId)
            => await _db.Itineraries
                .Include(i => i.Activities)
                .Include(i => i.PackingItems)
                .AsNoTracking()
                .Where(i => i.UserId == userId)
                .OrderByDescending(i => i.StartDate)
                .ToListAsync();

        public async Task<Itinerary?> GetByIdAsync(int id)
            => await _db.Itineraries
                .Include(i => i.Activities)
                .Include(i => i.PackingItems)
                .FirstOrDefaultAsync(i => i.Id == id);

        public async Task<Itinerary> CreateAsync(string userId, CreateItineraryDto dto)
        {
            var itinerary = new Itinerary
            {
                UserId = userId,
                Title = dto.Title,
                DestinationId = dto.DestinationId,
                Travelers = dto.Travelers,
                TripType = dto.TripType,
                Interests = dto.Interests,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };
            return await CreateAsync(itinerary);
        }

        public async Task<Itinerary> CreateAsync(Itinerary itinerary)
        {
            _db.Itineraries.Add(itinerary);
            await _db.SaveChangesAsync();
            return itinerary;
        }

        public async Task<Itinerary> SaveAsync(Itinerary itinerary)
        {
            if (itinerary.Id == 0) _db.Itineraries.Add(itinerary);
            else _db.Itineraries.Update(itinerary);
            await _db.SaveChangesAsync();
            return itinerary;
        }

        public async Task<Itinerary?> UpdateAsync(int id, UpdateItineraryDto dto)
        {
            var i = await _db.Itineraries.FindAsync(id);
            if (i is null) return null;

            if (dto.Title is not null) i.Title = dto.Title;
            if (dto.Travelers is not null) i.Travelers = dto.Travelers.Value;
            if (dto.TripType is not null) i.TripType = dto.TripType;
            if (dto.Interests is not null) i.Interests = dto.Interests;
            if (dto.StartDate is not null) i.StartDate = dto.StartDate.Value;
            if (dto.EndDate is not null) i.EndDate = dto.EndDate.Value;

            await _db.SaveChangesAsync();
            return i;
        }

        public async Task<Itinerary?> UpdateAsync(int id, string userId, string title, DateTime startDate, DateTime endDate)
        {
            var i = await _db.Itineraries.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
            if (i is null) return null;
            i.Title = title;
            i.StartDate = startDate;
            i.EndDate = endDate;
            await _db.SaveChangesAsync();
            return i;
        }

        public async Task DeleteAsync(int id)
        {
            var i = await _db.Itineraries.FindAsync(id);
            if (i is null) return;
            _db.Itineraries.Remove(i);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id, string userId)
        {
            var i = await _db.Itineraries.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
            if (i is null) return false;
            _db.Itineraries.Remove(i);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddActivityAsync(int itineraryId, DailyActivity activity)
        {
            if (await _db.Itineraries.FindAsync(itineraryId) is null) return false;
            activity.ItineraryId = itineraryId;
            _db.DailyActivities.Add(activity);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddPackingItemAsync(int itineraryId, PackingItem item)
        {
            if (await _db.Itineraries.FindAsync(itineraryId) is null) return false;
            item.ItineraryId = itineraryId;
            _db.PackingItems.Add(item);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> TogglePackingItemAsync(int itemId)
        {
            var item = await _db.PackingItems.FindAsync(itemId);
            if (item is null) return false;
            item.IsPacked = !item.IsPacked;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemovePackingItemAsync(int itemId)
        {
            var item = await _db.PackingItems.FindAsync(itemId);
            if (item is null) return false;
            _db.PackingItems.Remove(item);
            await _db.SaveChangesAsync();
            return true;
        }
    }

    public record CreateItineraryDto(
        string Title,
        int DestinationId,
        int Travelers,
        string TripType,
        List<string> Interests,
        DateTime StartDate,
        DateTime EndDate
    );

    public record UpdateItineraryDto(
        string? Title,
        int? Travelers,
        string? TripType,
        List<string>? Interests,
        DateTime? StartDate,
        DateTime? EndDate
    );
}