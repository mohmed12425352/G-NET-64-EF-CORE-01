using EventHub.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventHub.Configurations
{
    // =========================================================================
    // TECHNIQUE 2: SEPARATE CONFIGURATION CLASS (IEntityTypeConfiguration)
    // =========================================================================
    public class AttendeeConfiguration : IEntityTypeConfiguration<Attendee>
    {
        public void Configure(EntityTypeBuilder<Attendee> builder)
        {
            builder.ToTable("Attendees");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.FullName)
                   .IsRequired()
                   .HasMaxLength(120);

            builder.Property(a => a.Email)
                   .IsRequired()
                   .HasMaxLength(150);

            // Configure Owned Entity Type (Value Object: Address)
            builder.OwnsOne(a => a.HomeAddress, addr =>
            {
                addr.Property(p => p.Street).HasColumnName("Street").HasMaxLength(150).IsRequired();
                addr.Property(p => p.City).HasColumnName("City").HasMaxLength(80).IsRequired();
                addr.Property(p => p.Country).HasColumnName("Country").HasMaxLength(80).IsRequired();
                addr.Property(p => p.PostalCode).HasColumnName("PostalCode").HasMaxLength(20).IsRequired();
            });

            // 1-to-1 relationship with Badge
            builder.HasOne(a => a.Badge)
                   .WithOne(b => b.Attendee)
                   .HasForeignKey<Badge>(b => b.AttendeeId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
