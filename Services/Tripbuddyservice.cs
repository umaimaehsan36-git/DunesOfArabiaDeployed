using Microsoft.EntityFrameworkCore;
using DunesOfArabia.Data;
using DunesOfArabia.Models;

namespace DunesOfArabia.Services
{
    public interface ITripBuddyService
    {
        Task<List<TripBuddyPost>> GetAllPostsAsync();
        Task<List<TripBuddyPost>> GetPostsByDestinationAsync(string destinationName);
        Task<TripBuddyPost> CreatePostAsync(TripBuddyPost post);
        Task DeletePostAsync(int postId, string userId);
        Task<TripBuddyJoinRequest> SendJoinRequestAsync(int postId, string requesterId, string requesterName);
        Task UpdateJoinRequestAsync(int requestId, string status, string ownerId);
        Task<List<TripBuddyJoinRequestDto>> GetRequestsForOwnerAsync(string ownerId);
        Task<List<TripBuddyJoinRequest>> GetMyRequestsAsync(string requesterId);
        Task<List<TripBuddyChatMessage>> GetMessagesAsync(int postId, string viewerId, string otherUserId);
        Task<TripBuddyChatMessage> SendMessageAsync(int postId, string senderId, string senderName, string recipientId, string text);
    }

    /// <summary>Flat DTO carrying the requester's display name for the owner's inbox.</summary>
    public class TripBuddyJoinRequestDto
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string RequesterId { get; set; } = "";
        public string RequesterName { get; set; } = "";
        public string Status { get; set; } = "Pending";
        public DateTime RequestedAt { get; set; }
    }

    public class TripBuddyService : ITripBuddyService
    {
        private readonly AppDbContext _db;
        public TripBuddyService(AppDbContext db) { _db = db; }

        public async Task<List<TripBuddyPost>> GetAllPostsAsync()
            => await _db.TripBuddyPosts
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

        public async Task<List<TripBuddyPost>> GetPostsByDestinationAsync(string destinationName)
            => await _db.TripBuddyPosts
                .AsNoTracking()
                .Where(p => p.DestinationName == destinationName)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

        public async Task<TripBuddyPost> CreatePostAsync(TripBuddyPost post)
        {
            post.CreatedAt = DateTime.UtcNow;
            _db.TripBuddyPosts.Add(post);
            await _db.SaveChangesAsync();
            return post;
        }

        public async Task DeletePostAsync(int postId, string userId)
        {
            var post = await _db.TripBuddyPosts
                .FirstOrDefaultAsync(p => p.Id == postId && p.UserId == userId);
            if (post is null) return;
            _db.TripBuddyPosts.Remove(post);
            await _db.SaveChangesAsync();
        }

        public async Task<TripBuddyJoinRequest> SendJoinRequestAsync(int postId, string requesterId, string requesterName)
        {
            // Prevent duplicate requests from the same user on the same post
            var existing = await _db.TripBuddyJoinRequests
                .FirstOrDefaultAsync(r => r.PostId == postId && r.RequesterId == requesterId);
            if (existing != null) return existing;

            var req = new TripBuddyJoinRequest
            {
                PostId = postId,
                RequesterId = requesterId,
                RequesterName = requesterName,
                Status = "Pending",
                RequestedAt = DateTime.UtcNow
            };
            _db.TripBuddyJoinRequests.Add(req);
            await _db.SaveChangesAsync();
            return req;
        }

        public async Task UpdateJoinRequestAsync(int requestId, string status, string ownerId)
        {
            var req = await _db.TripBuddyJoinRequests.FindAsync(requestId);
            if (req is null) return;

            req.Status = status;
            if (status == "Accepted")
            {
                var post = await _db.TripBuddyPosts.FindAsync(req.PostId);
                if (post != null && post.SpotsLeft > 0) post.SpotsLeft--;
            }
            await _db.SaveChangesAsync();
        }

        public async Task<List<TripBuddyJoinRequestDto>> GetRequestsForOwnerAsync(string ownerId)
        {
            var myPostIds = await _db.TripBuddyPosts
                .Where(p => p.UserId == ownerId)
                .Select(p => p.Id)
                .ToListAsync();

            if (!myPostIds.Any()) return new List<TripBuddyJoinRequestDto>();

            var requests = await _db.TripBuddyJoinRequests
                .AsNoTracking()
                .Where(r => myPostIds.Contains(r.PostId))
                .OrderByDescending(r => r.RequestedAt)
                .ToListAsync();

            return requests.Select(r => new TripBuddyJoinRequestDto
            {
                Id = r.Id,
                PostId = r.PostId,
                RequesterId = r.RequesterId,
                RequesterName = string.IsNullOrEmpty(r.RequesterName) ? r.RequesterId : r.RequesterName,
                Status = r.Status,
                RequestedAt = r.RequestedAt,
            }).ToList();
        }

        public async Task<List<TripBuddyJoinRequest>> GetMyRequestsAsync(string requesterId)
            => await _db.TripBuddyJoinRequests
                .AsNoTracking()
                .Where(r => r.RequesterId == requesterId)
                .ToListAsync();

        public async Task<List<TripBuddyChatMessage>> GetMessagesAsync(int postId, string viewerId, string otherUserId)
            => await _db.TripBuddyMessages
                .AsNoTracking()
                .Where(m => m.PostId == postId &&
                            ((m.SenderId == viewerId && m.RecipientId == otherUserId) ||
                             (m.SenderId == otherUserId && m.RecipientId == viewerId)))
                .OrderBy(m => m.SentAt)
                .ToListAsync();

        public async Task<TripBuddyChatMessage> SendMessageAsync(
            int postId, string senderId, string senderName, string recipientId, string text)
        {
            var msg = new TripBuddyChatMessage
            {
                PostId = postId,
                SenderId = senderId,
                SenderName = senderName,
                RecipientId = recipientId,
                Text = text,
                SentAt = DateTime.UtcNow
            };
            _db.TripBuddyMessages.Add(msg);
            await _db.SaveChangesAsync();
            return msg;
        }
    }
}