namespace Application.Domain.ValueObjects;

public record MessageId(Guid Val) : BaseGuidId(Val);