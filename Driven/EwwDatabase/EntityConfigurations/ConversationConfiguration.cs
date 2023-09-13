using Application.Domain.Entities;
using Application.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EwwDatabase.EntityConfigurations;

public class ConversationConfiguration : BaseEntityConfiguration<Conversation, ConversationId>
{
    public override void Configure(EntityTypeBuilder<Conversation> builder)
    {
        base.Configure(builder);

        builder
            .HasIndex(c => c.CreatorUserId);
        
        builder
            .Property(c => c.CreatorUserId)
            .HasConversion(
                creatorUserId => creatorUserId.Val,
                creatorUserIdVal => new UserId(creatorUserIdVal))
            .HasColumnType("varchar");

        builder
            .Property(c => c.Type)
            .HasConversion<int>();
        
        builder
            .Property(c => c.Name)
            .HasConversion(
                name => name.Val,
                nameVal => new ConversationName(nameVal))
            .IsRequired(false)
            .HasColumnType("varchar");

        builder
            .HasMany(c => c.MemberList)
            .WithOne(m => m.Conversation)
            .HasForeignKey(m => m.ConversationId);
    }
}