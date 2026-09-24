using System;
using System.Collections.Generic;

namespace EventHub.Models
{
    // POCO - Configured directly in OnModelCreating via Fluent API (Technique 3)
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int MaxAttendees { get; set; }

        // Foreign Key to Organizer
        public int OrganizerId { get; set; }
        public Organizer Organizer { get; set; } = null!;

        // Self-referencing relationship: Parent Conference Event -> Breakout Sessions
        public int? ParentEventId { get; set; }
        public Event? ParentEvent { get; set; }
        public ICollection<Event> Sessions { get; set; } = new List<Event>();

        // Registrations
        public ICollection<EventRegistration> Registrations { get; set; } = new List<EventRegistration>();

        // NOTE: Shadow Properties 'CreatedAt' and 'LastModified' are configured
        // in DbContext OnModelCreating and not present as C# class properties!
    }
}
