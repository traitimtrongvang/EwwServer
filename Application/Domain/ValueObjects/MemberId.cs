namespace Application.Domain.ValueObjects;

public record MemberId(Guid Val) : BaseGuidId(Val);