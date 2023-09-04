namespace Application.Domain.ValueObjects;

public record ConversationId(Guid Val) : BaseGuidId(Val);