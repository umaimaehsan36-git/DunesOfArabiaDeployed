// DunesOfArabia/Models/Booking.cs
// ─────────────────────────────────────────────────────────────────────────────
// CHANGES vs your original:
//   1. BookingDocument.FileUrl added — DocumentUploadService and
//      BookingConfirmation.razor both reference doc.FileUrl.
//      Your original only had FilePath; adding FileUrl as the preferred
//      property and keeping FilePath as an alias prevents any break.
// ─────────────────────────────────────────────────────────────────────────────

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DunesOfArabia.Models
{
    public class Booking
    {
        public int    Id              { get; set; }

        [Required]
        public string UserId          { get; set; } = string.Empty;
        public int    DestinationId   { get; set; }

        public DateTime StartDate     { get; set; }
        public DateTime EndDate       { get; set; }

        [MaxLength(30)]
        public string Status          { get; set; } = "Pending";
        // Pending | Confirmed | Completed | Cancelled | Refunded

        [MaxLength(100)]
        public string StripePaymentIntentId { get; set; } = string.Empty;

        [MaxLength(30)]
        public string ConfirmationNumber    { get; set; } = string.Empty;

        // ── Timestamps ────────────────────────────────────────────────────────
        public DateTime CreatedAt   { get; set; } = DateTime.UtcNow;

        // Alias — Razor components that use CreatedDate still compile.
        // Alias — Razor components that use CreatedDate still compile.
        [NotMapped]
        public DateTime CreatedDate => CreatedAt;

        // ── Travelers & Price breakdown

        // ── Travelers & Price breakdown ───────────────────────────────────────
        public int     NumberOfTravelers { get; set; } = 1;

        public decimal TotalPrice { get; set; }
        public decimal Subtotal   { get; set; }
        public decimal Tax        { get; set; }

        [MaxLength(30)]
        public string PaymentMethod { get; set; } = string.Empty;

        // ── Navigation ────────────────────────────────────────────────────────
        public Destination?          Destination { get; set; }
        public List<BookingDocument> Documents   { get; set; } = new();
    }


    public class BookingDocument
    {
        public int    Id         { get; set; }
        public int    BookingId  { get; set; }

        [MaxLength(200)]
        public string FileName   { get; set; } = string.Empty;

        // FIX: FileUrl — BookingConfirmation.razor and DocumentUploadService
        // reference doc.FileUrl. Your original only had FilePath.
        // Both are stored so nothing breaks:
        //   FileUrl  → relative URL served by the app  e.g. "/uploads/..."
        //   FilePath → physical path on disk            e.g. "C:\wwwroot\uploads\..."
        [MaxLength(500)]
        public string? FileUrl   { get; set; }

        [MaxLength(500)]
        public string FilePath   { get; set; } = string.Empty;

        [MaxLength(10)]
        public string FileType   { get; set; } = string.Empty;

        // ── Timestamps ────────────────────────────────────────────────────────
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Alias — Razor components that use CreatedDate still compile.
        [NotMapped]
        public DateTime CreatedDate => CreatedAt;

        // ── Navigation ────────────────────────────────────────────────────────
        public Booking Booking   { get; set; } = null!;
    }
}
