using System;
using EventHub.Configurations;
using EventHub.Models;
using Microsoft.EntityFrameworkCore;

namespace EventHub.Data
{
    public class EventHubDbContext : DbContext
    {
        public DbSet<Organizer> Organizers { get; set; } = null!;
        public DbSet<OrganizerProfile> OrganizerProfiles { get; set; } = null!;
        public DbSet<Attendee> Attendees { get; set; } = null!;
        public DbSet<Badge> Badges { get; set; } = null!;
        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<EventRegistration> EventRegistrations { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=EventHubDB;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =====================================================================
            // TECHNIQUE 2: Apply Separate Configuration Classes
            // (Applied to Attendee, Address owned type, and Badge)
            // =====================================================================
            modelBuilder.ApplyConfiguration(new AttendeeConfiguration());
            modelBuilder.ApplyConfiguration(new BadgeConfiguration());

            // =====================================================================
            // TECHNIQUE 3: FLUENT API DIRECTLY IN ONMODELCREATING
            // (Applied to Event and EventRegistration)
            // =====================================================================
            
            // 1. Configure Event Entity
            modelBuilder.Entity<Event>(eb =>
            {
                eb.ToTable("Events");

                eb.HasKey(e => e.Id);

                eb.Property(e => e.Title)
                  .IsRequired()
                  .HasMaxLength(200);

                eb.Property(e => e.Description)
                  .IsRequired()
                  .HasMaxLength(2000);

                eb.Property(e => e.StartDate)
                  .IsRequired();

                eb.Property(e => e.EndDate)
                  .IsRequired(false);

                eb.Property(e => e.MaxAttendees)
                  .IsRequired();

                // Shadow Properties: CreatedAt and LastModified
                eb.Property<DateTime>("CreatedAt")
                  .IsRequired();

                eb.Property<DateTime>("LastModified")
                  .IsRequired();

                // Self-Referencing 1-to-many: Conference Event -> Breakout Sessions
                eb.HasOne(e => e.ParentEvent)
                  .WithMany(e => e.Sessions)
                  .HasForeignKey(e => e.ParentEventId)
                  .OnDelete(DeleteBehavior.Restrict);

                // Organizer relationship
                eb.HasOne(e => e.Organizer)
                  .WithMany(o => o.Events)
                  .HasForeignKey(e => e.OrganizerId)
                  .OnDelete(DeleteBehavior.Cascade);
            });

            // 2. Configure EventRegistration (Many-to-Many Junction Entity with payload)
            modelBuilder.Entity<EventRegistration>(eb =>
            {
                eb.ToTable("EventRegistrations");

                // Composite Primary Key (EventId + AttendeeId)
                eb.HasKey(er => new { er.EventId, er.AttendeeId });

                eb.Property(er => er.RegistrationDate)
                  .IsRequired();

                eb.Property(er => er.Notes)
                  .HasMaxLength(500)
                  .IsRequired(false);

                // Relationship: EventRegistration -> Event
                eb.HasOne(er => er.Event)
                  .WithMany(e => e.Registrations)
                  .HasForeignKey(er => er.EventId)
                  .OnDelete(DeleteBehavior.Cascade);

                // Relationship: EventRegistration -> Attendee
                eb.HasOne(er => er.Attendee)
                  .WithMany(a => a.Registrations)
                  .HasForeignKey(er => er.AttendeeId)
                  .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
