using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TouristsCore.Entities;

namespace TouristsRepository.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.Property(b => b.TotalPrice).HasPrecision(18, 2);
        
        // Tourist -> Booking
        // Rule:  You cannot delete a User if they have bookings. (Keeps History)
        builder.HasOne(b => b.Tourist)
            .WithMany()
            .HasForeignKey(b => b.TouristId)
            .OnDelete(DeleteBehavior.Restrict); 

        
        // Tour -> Booking
        // Rule: You cannot delete a Tour if it has active bookings.
        builder.HasOne(b => b.Tour)
            .WithMany(t => t.Bookings)
            .HasForeignKey(b => b.TourId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}