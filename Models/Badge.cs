using System;

namespace EventHub.Models
{
    // POCO - Configured via BadgeConfiguration (Technique 2)
    public class Badge
    {
        public int Id { get; set; }
        public string BadgeNumber { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public BadgeTier Tier { get; set; }

        // 1-to-1 optional relationship with Attendee
        public int AttendeeId { get; set; }
        public Attendee Attendee { get; set; } = null!;
    }
}
