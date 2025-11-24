using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TouristsCore.Entities;

namespace TouristsRepository.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasOne(u => u.GuideProfile)
            .WithOne(u => u.User)
            .HasForeignKey<GuideProfile>(g => g.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(u => u.TouristProfile)
            .WithOne(u => u.User)
            .HasForeignKey<TouristProfile>(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}