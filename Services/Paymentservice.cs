using DunesOfArabia.Data;
using DunesOfArabia.Models;
using Microsoft.EntityFrameworkCore;

namespace DunesOfArabia.Services
{
    // ═══════════════════════════════════════════════════════
    // PAYMENT SERVICE
    // Stub implementation — wire up Stripe later by replacing
    // the method bodies. The interface is already consumed by
    // Blazor pages (Checkout.razor, BookingConfirmation.razor).
    // ═══════════════════════════════════════════════════════

    public interface IPaymentService
    {
        /// <summary>Creates a payment intent and returns the client secret.</summary>
        Task<PaymentIntentResult> CreatePaymentIntentAsync(int bookingId, string userId);

        /// <summary>Confirms the payment and marks the booking as Confirmed.</summary>
        Task<bool> ConfirmPaymentAsync(string paymentIntentId);

        /// <summary>Refunds the payment and marks the booking as Refunded.</summary>
        Task<bool> RefundAsync(string paymentIntentId);
    }

    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _db;
        private readonly IBookingService _bookingService;

        public PaymentService(AppDbContext db, IBookingService bookingService)
        {
            _db = db;
            _bookingService = bookingService;
        }

        public async Task<PaymentIntentResult> CreatePaymentIntentAsync(int bookingId, string userId)
        {
            var booking = await _bookingService.GetByIdAsync(bookingId);
            if (booking is null || booking.UserId != userId)
                return new PaymentIntentResult { Success = false, Error = "Booking not found." };

            // ── TODO: Replace with real Stripe call ──────────────────────
            // var options = new PaymentIntentCreateOptions { Amount = (long)(booking.TotalPrice * 100), Currency = "sar" };
            // var service = new PaymentIntentService();
            // var intent  = await service.CreateAsync(options);
            // booking.StripePaymentIntentId = intent.Id;
            // ────────────────────────────────────────────────────────────

            // Stub: generate a fake intent ID so pages don't crash
            var fakeIntentId = $"pi_stub_{bookingId}_{DateTime.UtcNow.Ticks}";
            booking.StripePaymentIntentId = fakeIntentId;
            await _db.SaveChangesAsync();

            return new PaymentIntentResult
            {
                Success = true,
                IntentId = fakeIntentId,
                ClientSecret = $"{fakeIntentId}_secret_stub"
            };
        }

        public async Task<bool> ConfirmPaymentAsync(string paymentIntentId)
        {
            var booking = await _db.Bookings
                .FirstOrDefaultAsync(b => b.StripePaymentIntentId == paymentIntentId);
            if (booking is null) return false;

            booking.Status = "Confirmed";
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RefundAsync(string paymentIntentId)
        {
            var booking = await _db.Bookings
                .FirstOrDefaultAsync(b => b.StripePaymentIntentId == paymentIntentId);
            if (booking is null) return false;

            // TODO: call Stripe Refunds API here
            booking.Status = "Refunded";
            await _db.SaveChangesAsync();
            return true;
        }
    }

    public class PaymentIntentResult
    {
        public bool Success { get; set; }
        public string? IntentId { get; set; }
        public string? ClientSecret { get; set; }
        public string? Error { get; set; }
    }
}