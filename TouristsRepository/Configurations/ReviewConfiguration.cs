using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TouristsCore.Entities;

namespace TouristsRepository.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasIndex(r => r.BookingId).IsUnique(); // 1 booking -> 1 review 
        // Booking -> Review
        // Rule: If Booking is deleted, Review is deleted.
        builder.HasOne(r => r.Booking)
            .WithOne()
            .HasForeignKey<Review>(r => r.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        // Guide -> Review
        // Rule: Restrict. Don't let review deletion logic crash with User deletion logic.
        builder.HasOne(r => r.Guide)
            .WithMany()
            .HasForeignKey(r => r.GuideId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}