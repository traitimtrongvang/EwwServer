using Application.Domain.Entities;
using Application.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EwwDatabase.EntityConfigurations;

public abstract class BaseEntityConfiguration<TEntity, TId> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity<TId>
    where TId : BaseGuidId
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder
            .HasKey(e => e.Id);

        builder
            .Property(e => e.Id)
            .HasConversion(
                id => id.Val,
                idVal => (TId)Activator.CreateInstance(typeof(TId), idVal)!);

        builder
            .Property(e => e.CreatedAt)
            .HasColumnType("timestamp");
        
        builder
            .Property(e => e.Updated)
            .IsRequired(false)
            .HasColumnType("timestamp");
    }
}