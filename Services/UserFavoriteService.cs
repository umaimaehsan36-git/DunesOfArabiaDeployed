using Microsoft.EntityFrameworkCore;
using DunesOfArabia.Data;
using DunesOfArabia.Models;

namespace DunesOfArabia.Services
{
    public interface IUserFavoriteService
    {
        Task<List<UserFavorite>> GetUserFavoritesAsync(string userId);
        Task<List<UserFavorite>> GetFavoritesByUserAsync(string userId);
        Task<bool> AddFavoriteAsync(string userId, int destinationId);
        Task<bool> RemoveFavoriteAsync(string userId, int destinationId);
        Task<bool> IsFavoriteAsync(string userId, int destinationId);
    }

    public class UserFavoriteService : IUserFavoriteService
    {
        private readonly AppDbContext _db;
        public UserFavoriteService(AppDbContext db) { _db = db; }

        public async Task<List<UserFavorite>> GetUserFavoritesAsync(string userId)
            => await _db.UserFavorites
                .AsNoTracking()
                .Where(f => f.UserId == userId)
                .ToListAsync();

        public Task<List<UserFavorite>> GetFavoritesByUserAsync(string userId)
            => GetUserFavoritesAsync(userId);

        public async Task<bool> AddFavoriteAsync(string userId, int destinationId)
        {
            if (await _db.UserFavorites.AnyAsync(f => f.UserId == userId && f.DestinationId == destinationId))
                return false;
            _db.UserFavorites.Add(new UserFavorite { UserId = userId, DestinationId = destinationId });
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveFavoriteAsync(string userId, int destinationId)
        {
            var fav = await _db.UserFavorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.DestinationId == destinationId);
            if (fav is null) return false;
            _db.UserFavorites.Remove(fav);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsFavoriteAsync(string userId, int destinationId)
            => await _db.UserFavorites
                .AnyAsync(f => f.UserId == userId && f.DestinationId == destinationId);
    }
}