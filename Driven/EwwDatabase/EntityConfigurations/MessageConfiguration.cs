using Application.Domain.Entities;
using Application.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EwwDatabase.EntityConfigurations;

public class MessageConfiguration : BaseEntityConfiguration<Message, MessageId>
{
    public override void Configure(EntityTypeBuilder<Message> builder)
    {
        base.Configure(builder);

        builder
            .Property(m => m.SenderMemberId)
            .HasConversion(
                senderMemberId => senderMemberId.Val,
                senderMemberIdVal => new MemberId(senderMemberIdVal));

        builder
            .Property(m => m.Content)
            .HasConversion(
                content => content.Val,
                contentVal => new MessageContent(contentVal))
            .HasColumnType("varchar");

        builder
            .HasOne(m => m.SenderMember)
            .WithMany(m => m.MessageList)
            .HasForeignKey(m => m.SenderMemberId);
    }
}