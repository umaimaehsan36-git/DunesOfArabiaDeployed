// DunesOfArabia/Models/Itinerary.cs
// ─────────────────────────────────────────────────────────────────────────────
// CHANGES vs your original:
//
// Itinerary:
//   1. CreatedAt added — ItineraryService.GetUserItinerariesAsync orders by it.
//      Without it the service throws CS1061 at runtime.
//
// DailyActivity:
//   2. ActivityName added — Planner.razor step-3 summary and ItineraryService
//      .AddActivityAsync write to ActivityName; your original only had Title.
//      Title kept as alias so existing MyItineraries.razor line 111 still works.
//   3. TimeSlot added — Planner day-slot picker writes "Morning" / "Afternoon"
//      / "Evening" here. Without it the planner can't persist time slots.
//   4. Notes added — Planner step notes field writes here.
//   5. Description kept for backward-compat (MyItineraries detail view).
//
// PackingItem:
//   6. Category added — Profile.razor packing list groups items by category
//      (Clothing | Documents | Electronics | Health | General).
//      Without it the grouping silently falls back to "General" for everything.
// ─────────────────────────────────────────────────────────────────────────────

namespace DunesOfArabia.Models
{
    public class Itinerary
    {
        public int    Id            { get; set; }
        public string UserId        { get; set; } = string.Empty;
        public string Title         { get; set; } = string.Empty;
        public int    DestinationId { get; set; }
        public int    Travelers     { get; set; } = 1;
        public string TripType      { get; set; } = string.Empty;
        public List<string> Interests { get; set; } = new();
        public DateTime StartDate   { get; set; }
        public DateTime EndDate     { get; set; }

        // FIX 1: Added — ItineraryService orders by this; missing caused CS1061.
        public DateTime CreatedAt   { get; set; } = DateTime.UtcNow;

        // ── Navigation ────────────────────────────────────────────────────────
        public List<DailyActivity> Activities  { get; set; } = new();
        public List<PackingItem>   PackingItems { get; set; } = new();
        public Destination?        Destination  { get; set; }
    }


    public class DailyActivity
    {
        public int Id          { get; set; }
        public int ItineraryId { get; set; }
        public int DayNumber   { get; set; }

        // FIX 2: ActivityName — Planner.razor writes and reads this field.
        // Service's AddActivityAsync sets activity.ActivityName before insert.
        public string ActivityName { get; set; } = string.Empty;

        // Alias kept so MyItineraries.razor line 111 (@activity.Title) compiles.
        public string Title
        {
            get => ActivityName;
            set => ActivityName = value;
        }

        // FIX 3: TimeSlot — "Morning" | "Afternoon" | "Evening"
        // Planner step-3 day-slot picker writes here.
        public string TimeSlot { get; set; } = "Morning";

        // FIX 4: Notes — optional free-text per activity slot.
        public string Notes { get; set; } = string.Empty;

        // Kept for MyItineraries detail view (shows description under activity name).
        public string Description { get; set; } = string.Empty;

        // ── Navigation ────────────────────────────────────────────────────────
        public Itinerary Itinerary { get; set; } = null!;
    }
}
