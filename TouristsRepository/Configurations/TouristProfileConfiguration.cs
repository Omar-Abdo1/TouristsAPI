using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TouristsCore.Entities;

namespace TouristsRepository.Configurations;

public class TouristProfileConfiguration : IEntityTypeConfiguration<TouristProfile>
{
    public void Configure(EntityTypeBuilder<TouristProfile> builder)
    {
        builder.Property(t => t.FullName).HasMaxLength(100).IsRequired();
        builder.HasOne(t => t.AvatarFile)
            .WithOne()
            .HasForeignKey<TouristProfile>(t => t.AvatarFileId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}