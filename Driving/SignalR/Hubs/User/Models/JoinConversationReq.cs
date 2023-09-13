namespace SignalR.Hubs.User.Models;

public record JoinConversationReq
{
    public required Guid ConversationId { get; init; }
}