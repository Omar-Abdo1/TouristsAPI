using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TouristsCore.Entities;

namespace TouristsRepository.Configurations;

public class GuideLanguageConfiguration : IEntityTypeConfiguration<GuideLanguage>
{
    public void Configure(EntityTypeBuilder<GuideLanguage> builder)
    {
        builder.HasKey(gl => new { gl.GuideProfileId, gl.LanguageId });

        // Link: Guide -> Language
        builder.HasOne(gl => gl.GuideProfile)
            .WithMany(g => g.GuideLanguages)
            .HasForeignKey(gl => gl.GuideProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        // Link: Language -> Guide
        // can not delete language if Guid is using it
        builder.HasOne(gl => gl.Language)
            .WithMany(l => l.GuideLanguages)
            .HasForeignKey(gl => gl.LanguageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}