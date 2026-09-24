using System;

namespace EventHub.Models
{
    // POCO - Junction Entity configured directly in OnModelCreating via Fluent API (Technique 3)
    public class EventRegistration
    {
        public int EventId { get; set; }
        public Event Event { get; set; } = null!;

        public int AttendeeId { get; set; }
        public Attendee Attendee { get; set; } = null!;

        public DateTime RegistrationDate { get; set; }
        public string? Notes { get; set; }
    }
}
