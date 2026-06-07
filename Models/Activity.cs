namespace DunesOfArabia.Models
{
    public class Activity
    {
        public int Id { get; set; }
        public int DestinationId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;

        // ── Duration ──────────────────────────────────────────
        // DurationHours kept for service compatibility.
        // Duration (int, in minutes) added for Razor component usage.
        public decimal DurationHours { get; set; }
        public int Duration => (int)(DurationHours * 60); // computed in minutes

        // ── Pricing ───────────────────────────────────────────
        // PriceSAR kept for service compatibility.
        // Price added as alias used by Razor components.
        public decimal PriceSAR { get; set; }
        public decimal Price => PriceSAR;

        // ── Difficulty & Participants ─────────────────────────
        public string DifficultyLevel { get; set; } = "Easy";
        public int MaxParticipants { get; set; } = 20;
        public int MinAge { get; set; } = 0;

        // ── Rating ────────────────────────────────────────────
        public double Rating { get; set; } = 0;

        // ── Included Services & Policies ─────────────────────
        public string IncludedServices { get; set; } = string.Empty;
        public string CancellationPolicy { get; set; } = string.Empty;

        // ── Operator Info ─────────────────────────────────────
        public string OperatorName { get; set; } = string.Empty;
        public string OperatorEmail { get; set; } = string.Empty;
        public string OperatorPhone { get; set; } = string.Empty;

        // ── Navigation ────────────────────────────────────────
        public Destination? Destination { get; set; }
    }
}