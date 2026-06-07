// DunesOfArabia/Models/PackingItem.cs
// ─────────────────────────────────────────────────────────────────────────────
// CHANGES vs your original:
//   1. Category added — Profile.razor packing list tab groups items by this.
//      Values: Clothing | Documents | Electronics | Health | General
//      Default is "General" so existing rows without a value still display.
// ─────────────────────────────────────────────────────────────────────────────

using System.ComponentModel.DataAnnotations;

namespace DunesOfArabia.Models
{
    public class PackingItem
    {
        public int  Id          { get; set; }
        public int  ItineraryId { get; set; }

        [Required, MaxLength(150)]
        public string ItemName  { get; set; } = string.Empty;

        public bool IsPacked    { get; set; } = false;

        // FIX: Category — Profile.razor groups packing items by this.
        // Valid values: Clothing | Documents | Electronics | Health | General
        [MaxLength(30)]
        public string Category  { get; set; } = "General";

        // ── Navigation ────────────────────────────────────────────────────────
        public Itinerary? Itinerary { get; set; }
    }
}
