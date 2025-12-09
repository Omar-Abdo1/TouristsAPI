using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TouristsCore.Entities;

namespace TouristsRepository.Configurations;

public class FileRecordConfiguration : IEntityTypeConfiguration<FileRecord>
{
    public void Configure(EntityTypeBuilder<FileRecord> builder)
    {
        builder.Property(f => f.Size).IsRequired();
        
        // Owner -> File
        // Rule: Restrict. We don't want files disappearing if a user is deleted 
        // until we confirm the storage is cleaned up.
        builder.HasOne(f => f.User)
            .WithMany()
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.SetNull);
        //todo
        // Find files that:
        // 1. Have NO owner
        // 2. Are NOT used in a Message 
        // 3. Are NOT used in a Tourist Profile 
        // 4. Were created more than a week ago 
    }
}