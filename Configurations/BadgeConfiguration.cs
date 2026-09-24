using EventHub.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventHub.Configurations
{
    // =========================================================================
    // TECHNIQUE 2: SEPARATE CONFIGURATION CLASS (IEntityTypeConfiguration)
    // =========================================================================
    public class BadgeConfiguration : IEntityTypeConfiguration<Badge>
    {
        public void Configure(EntityTypeBuilder<Badge> builder)
        {
            builder.ToTable("Badges");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.BadgeNumber)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasIndex(b => b.BadgeNumber)
                   .IsUnique();

            builder.Property(b => b.IssueDate)
                   .IsRequired();

            // Store enum as string
            builder.Property(b => b.Tier)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();
        }
    }
}
