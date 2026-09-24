using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventHub.Models
{
    // =========================================================================
    // TECHNIQUE 1: DATA ANNOTATIONS
    // Configured exclusively with Data Annotations attributes
    // =========================================================================
    [Table("OrganizerProfiles")]
    public class OrganizerProfile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(600)]
        public string Bio { get; set; } = string.Empty;

        [MaxLength(200)]
        [Url]
        public string? Website { get; set; }

        [MaxLength(250)]
        public string? LogoUrl { get; set; }

        // Required Foreign Key to Organizer (1-to-1)
        [ForeignKey("Organizer")]
        public int OrganizerId { get; set; }

        public virtual Organizer Organizer { get; set; } = null!;
    }
}
