using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventHub.Models
{
    // =========================================================================
    // TECHNIQUE 1: DATA ANNOTATIONS
    // Configured exclusively with Data Annotations attributes
    // =========================================================================
    [Table("Organizers")]
    public class Organizer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(120)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? CompanyName { get; set; }

        public bool IsVerified { get; set; }

        // 1-to-1 required relationship with OrganizerProfile
        public virtual OrganizerProfile Profile { get; set; } = null!;

        // 1-to-many relationship with Event
        [InverseProperty("Organizer")]
        public virtual ICollection<Event> Events { get; set; } = new List<Event>();
    }
}
