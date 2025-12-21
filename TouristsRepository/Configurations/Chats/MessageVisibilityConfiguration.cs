using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TouristsCore.Entities;

namespace TouristsRepository.Configurations;

public class MessageVisibilityConfiguration : IEntityTypeConfiguration<MessageVisibility>
{
    public void Configure(EntityTypeBuilder<MessageVisibility> builder)
    {
        builder.HasKey(mv => new { mv.MessageId, mv.UserId });
        
        builder.HasIndex(dm => new { dm.MessageId, dm.UserId })
            .IsUnique(); 
        // to know faster if a user has a row there or not

        // Message -> Visibility
        // Rule: Cascade. Delete Message = Delete Visibility records.
        builder.HasOne(mv => mv.Message)
            .WithMany(m => m.HiddenForUsers)
            .HasForeignKey(mv => mv.MessageId)
            .OnDelete(DeleteBehavior.Cascade);

        // User -> Visibility
        // Rule: Restrict. Prevents cycles.
        builder.HasOne(mv => mv.User)
            .WithMany()
            .HasForeignKey(mv => mv.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}