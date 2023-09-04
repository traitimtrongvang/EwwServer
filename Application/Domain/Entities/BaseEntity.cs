using Application.Domain.ValueObjects;

namespace Application.Domain.Entities;

public abstract class BaseEntity<TId> where TId : BaseGuidId 
{
    public required TId Id { get; init; }

    public DateTime CreatedAt { get; init; } = DateTime.Now;

    public DateTime? Updated { get; set; } = default;
}