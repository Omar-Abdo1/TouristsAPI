using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TouristsCore.Entities;

namespace TouristsRepository.Configurations;

public class ChatConfiguration : IEntityTypeConfiguration<Chat>
{
    public void Configure(EntityTypeBuilder<Chat> builder)
    {
        builder.HasOne(c=>c.LastMessage)
            .WithMany()
            .HasForeignKey(c=>c.LastMessageId)
            .OnDelete(DeleteBehavior.ClientSetNull);
    }
}