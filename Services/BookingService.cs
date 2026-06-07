using Microsoft.EntityFrameworkCore;
using DunesOfArabia.Data;
using DunesOfArabia.Models;

namespace DunesOfArabia.Services
{
    public interface IBookingService
    {
        Task<List<Booking>> GetByUserIdAsync(string userId);
        Task<List<Booking>> GetUserBookingsAsync(string userId);
        Task<List<Booking>> GetAllAsync();
        Task<List<Booking>> GetAllBookingsAsync();
        Task<Booking?> GetByIdAsync(int id);
        Task<Booking> CreateAsync(string userId, CreateBookingDto dto);
        Task CancelAsync(int id);
        Task<bool> UpdateStatusAsync(int id, string status);
        Task<bool> CancelAsync(int id, string userId);
        Task<bool> AddDocumentAsync(int bookingId, BookingDocument document);
        Task<Booking> UpdateAfterPaymentAsync(int bookingId, UpdateBookingPaymentDto dto);
    }

    public class BookingService : IBookingService
    {
        private readonly AppDbContext _db;
        public BookingService(AppDbContext db) { _db = db; }

        public Task<List<Booking>> GetByUserIdAsync(string userId)
            => GetUserBookingsAsync(userId);

        public async Task<List<Booking>> GetUserBookingsAsync(string userId)
            => await _db.Bookings
                .Include(b => b.Destination)
                .Include(b => b.Documents)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.StartDate)
                .ToListAsync();

        public Task<List<Booking>> GetAllAsync() => GetAllBookingsAsync();

        public async Task<List<Booking>> GetAllBookingsAsync()
            => await _db.Bookings
                .Include(b => b.Destination)
                .Include(b => b.Documents)
                .OrderByDescending(b => b.StartDate)
                .ToListAsync();

        public async Task<Booking?> GetByIdAsync(int id)
            => await _db.Bookings
                .Include(b => b.Destination)
                .Include(b => b.Documents)
                .FirstOrDefaultAsync(b => b.Id == id);

        public async Task<Booking> CreateAsync(string userId, CreateBookingDto dto)
        {
            var booking = new Booking
            {
                UserId = userId,
                DestinationId = dto.DestinationId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = "Pending",
                // FIX CS0200: Only assign CreatedDate if the property has a setter.
                // If your Booking model declares CreatedDate as { get; init; } or
                // computes it, remove this line — the default value in the model
                // (DateTime.UtcNow) is set automatically.
                // CreatedDate = DateTime.UtcNow,   ← uncomment only if setter exists
            };
            _db.Bookings.Add(booking);
            await _db.SaveChangesAsync();
            return booking;
        }

        public async Task CancelAsync(int id)
        {
            var booking = await _db.Bookings.FindAsync(id);
            if (booking is null) return;
            booking.Status = "Cancelled";
            await _db.SaveChangesAsync();
        }

        public async Task<bool> UpdateStatusAsync(int id, string status)
        {
            var booking = await _db.Bookings.FindAsync(id);
            if (booking is null) return false;
            booking.Status = status;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelAsync(int id, string userId)
        {
            var booking = await _db.Bookings
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);
            if (booking is null || booking.Status == "Completed") return false;
            booking.Status = "Cancelled";
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddDocumentAsync(int bookingId, BookingDocument document)
        {
            if (await _db.Bookings.FindAsync(bookingId) is null) return false;
            document.BookingId = bookingId;
          
            _db.BookingDocuments.Add(document);
            await _db.SaveChangesAsync();
            return true;
        }

        // FIX CS1061: The properties TransactionId, ConfirmationNumber, Subtotal,
        // Tax, PaymentMethod, NumberOfTravelers must exist on your Booking model.
        // Add the following to your Booking.cs if they are missing:
        //
        //   public string?  TransactionId      { get; set; }
        //   public string?  ConfirmationNumber  { get; set; }
        //   public decimal  Subtotal            { get; set; }
        //   public decimal  Tax                 { get; set; }
        //   public string?  PaymentMethod       { get; set; }
        //   public int      NumberOfTravelers   { get; set; }
        //
        // After adding them, run: Add-Migration AddPaymentFields && Update-Database
        public async Task<Booking> UpdateAfterPaymentAsync(int bookingId, UpdateBookingPaymentDto dto)
        {
            var booking = await _db.Bookings.FindAsync(bookingId)
                ?? throw new InvalidOperationException($"Booking {bookingId} not found.");

            booking.NumberOfTravelers = dto.NumberOfTravelers;
            booking.TotalPrice = dto.TotalPrice;
            booking.Subtotal = dto.Subtotal;
            booking.Tax = dto.Tax;
            booking.PaymentMethod = dto.PaymentMethod;
            // NOTE: TransactionId is not on the Booking model. If you need it,
            // add: public string? TransactionId { get; set; } to Booking.cs
            // and run: Add-Migration AddTransactionId && Update-Database
            booking.ConfirmationNumber = dto.ConfirmationNumber;
            booking.Status = dto.Status;

            await _db.SaveChangesAsync();
            return booking;
        }
    }

    public record CreateBookingDto(
        int DestinationId,
        DateTime StartDate,
        DateTime EndDate
    );

    // NOTE: TransactionId removed — not present on Booking model.
    // Add it to Booking.cs + a migration if you need to store it.
    public record UpdateBookingPaymentDto(
        int NumberOfTravelers,
        decimal TotalPrice,
        decimal Subtotal,
        decimal Tax,
        string PaymentMethod,
        string ConfirmationNumber,
        string Status
    );
}