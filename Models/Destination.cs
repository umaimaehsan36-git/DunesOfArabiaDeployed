// DunesOfArabia/Models/Destination.cs
// ─────────────────────────────────────────────────────────────────────────────
// CHANGES vs your original:
//   1. Region property — [NotMapped] alias for Province.
//      AdminDashboard.razor DestVM uses d.Region; no new DB column needed.
//   2. Price property — [NotMapped] alias for Cost.
//      AdminDashboard.razor DestVM uses d.Price; no new DB column needed.
//   3. ImageGallery — your original had it [NotMapped] reading from
//      ImageGalleryJson which is correct. Kept exactly as-is.
//   4. Highlights — same pattern, kept as-is.
//   5. CreatedAt/CreatedDate dual-property kept as-is.
// ─────────────────────────────────────────────────────────────────────────────

using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace DunesOfArabia.Models
{
    public class Destination
    {
        public int    Id          { get; set; }
        public string Name        { get; set; } = string.Empty;
        public string Province    { get; set; } = string.Empty;
        public string Category    { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl    { get; set; } = string.Empty;
        public double Latitude    { get; set; }
        public double Longitude   { get; set; }
        public decimal Cost       { get; set; }
        public string  Climate    { get; set; } = string.Empty;
        public string  VisaInfo   { get; set; } = string.Empty;
        public double  Rating     { get; set; }

        // ── Season / Weather ──────────────────────────────────────────────────
        public string BestSeason   { get; set; } = string.Empty;
        public string Temperature  { get; set; } = string.Empty;

        // ── Dates ─────────────────────────────────────────────────────────────
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt
        {
            get => CreatedDate;
            set => CreatedDate = value;
        }

        // ── AdminDashboard aliases (NotMapped — no extra DB columns) ──────────
        /// <summary>
        /// AdminDashboard DestVM reads d.Region.
        /// Maps to Province — same data, different name used in admin views.
        /// </summary>
        [NotMapped]
        public string Region => Province;

        /// <summary>
        /// AdminDashboard DestVM reads d.Price.
        /// Maps to Cost — same data, different name used in admin views.
        /// </summary>
        [NotMapped]
        public decimal Price => Cost;

        // ── Gallery (JSON in DB, List<string> in code) ────────────────────────
        public string ImageGalleryJson { get; set; } = string.Empty;

        [NotMapped]
        public List<string> ImageGallery
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ImageGalleryJson)) return new();
                try { return JsonSerializer.Deserialize<List<string>>(ImageGalleryJson) ?? new(); }
                catch { return new(); }
            }
            set => ImageGalleryJson = JsonSerializer.Serialize(value);
        }

        // ── Highlights (JSON in DB, List<string> in code) ────────────────────
        public string HighlightsJson { get; set; } = string.Empty;

        [NotMapped]
        public List<string> Highlights
        {
            get
            {
                if (string.IsNullOrWhiteSpace(HighlightsJson)) return new();
                try { return JsonSerializer.Deserialize<List<string>>(HighlightsJson) ?? new(); }
                catch { return new(); }
            }
            set => HighlightsJson = JsonSerializer.Serialize(value);
        }
    }
}
