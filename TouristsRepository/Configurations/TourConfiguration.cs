using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TouristsCore.Entities;

namespace TouristsRepository.Configurations;

public class TourConfiguration : IEntityTypeConfiguration<Tour>
{
    public void Configure(EntityTypeBuilder<Tour> builder)
    {
        builder.Property(t => t.Price).HasPrecision(18, 2).IsRequired();
        builder.Property(t => t.Title).HasMaxLength(200).IsRequired();
        builder.Property(t => t.City).IsRequired();
        builder.Property(t => t.Country).IsRequired();

        // Guide -> Tour
        // Rule: If Guide is deleted, their Tours are deleted.
        builder.HasOne(t => t.GuideProfile)
            .WithMany(g => g.Tours)
            .HasForeignKey(t => t.GuideProfileId)
            .OnDelete(DeleteBehavior.Cascade); 
    }
}