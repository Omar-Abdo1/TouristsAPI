using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TouristsCore.Entities;

namespace TouristsRepository.Configurations;

public class GuideProfileConfiguration : IEntityTypeConfiguration<GuideProfile>
{
    public void Configure(EntityTypeBuilder<GuideProfile> builder)
    {
        builder.Property(g => g.RatePerHour).HasPrecision(18, 2); 
        
        builder.HasOne(g => g.AvatarFile)
            .WithOne()
            .HasForeignKey<GuideProfile>(g => g.AvatarFileId)
            .OnDelete(DeleteBehavior.SetNull);
        
    }
}