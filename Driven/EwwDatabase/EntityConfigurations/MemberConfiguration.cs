using Application.Domain.Entities;
using Application.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EwwDatabase.EntityConfigurations;

public class MemberConfiguration : BaseEntityConfiguration<Member, MemberId>
{
    public override void Configure(EntityTypeBuilder<Member> builder)
    {
        base.Configure(builder);

        builder
            .HasIndex(m => m.UserId);
        
        builder
            .Property(c => c.UserId)
            .HasConversion(
                userId => userId.Val,
                userIdVal => new UserId(userIdVal))
            .HasColumnType("varchar");
        
        builder
            .Property(m => m.ConversationId)
            .HasConversion(
                conversationId => conversationId.Val,
                conversationIdVal => new ConversationId(conversationIdVal));

        builder
            .Property(m => m.NickName)
            .HasConversion(
                nickName => nickName.Val,
                nickNameVal => new NickName(nickNameVal))
            .HasColumnType("varchar");

        builder
            .Property(m => m.IsAccepted);

        builder
            .HasOne(m => m.Conversation)
            .WithMany(c => c.MemberList)
            .HasForeignKey(m => m.ConversationId);

        builder
            .HasMany(m => m.MessageList)
            .WithOne(m => m.SenderMember)
            .HasForeignKey(m => m.SenderMemberId);
    }
}