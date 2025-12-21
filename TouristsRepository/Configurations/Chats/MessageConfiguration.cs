using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TouristsCore.Entities;

namespace TouristsRepository.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        // Chat -> Message
        // Rule:  Delete Chat = Delete All Messages.
        builder.HasOne(m => m.Chat)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

        // Sender -> Message
        // Rule: can not delete a user if it send message 
        // todo handle it manually
        builder.HasOne(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.ReplyToMessage) // message has one reply   ChildSide
            .WithMany(m=>m.ReplyToMessages) // message can have more than reply  ParentSide  can have multiple childers
            .HasForeignKey(m => m.ReplyToMessageId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}