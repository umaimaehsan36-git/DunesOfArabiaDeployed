namespace DunesOfArabia.Models
{
    public class Complaint
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;    // FIX: added – used by Complaints.razor lines 100, 221
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty; // FIX: added – used by Complaints.razor lines 104, 223
        public string Message { get; set; } = string.Empty;     // kept for backwards compatibility
        public string Status { get; set; } = "Open";
        public string Priority { get; set; } = "Normal";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ResolvedAt { get; set; }
        public string? AdminResponse { get; set; }
    }
}