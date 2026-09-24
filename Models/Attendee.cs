using System.Collections.Generic;

namespace EventHub.Models
{
    // POCO - Configured via AttendeeConfiguration (Technique 2)
    public class Attendee
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Owned Entity Type (Home Address)
        public Address HomeAddress { get; set; } = new Address();

        // 1-to-1 relationship with Badge (optional)
        public Badge? Badge { get; set; }

        // Many-to-many relationship with Event through EventRegistration
        public ICollection<EventRegistration> Registrations { get; set; } = new List<EventRegistration>();
    }
}
