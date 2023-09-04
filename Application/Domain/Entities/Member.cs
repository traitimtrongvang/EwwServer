using Application.Domain.ValueObjects;

namespace Application.Domain.Entities;

public class Member : BaseEntity<MemberId>
{
    public required UserId UserId { get; init; }

    public required ConversationId ConversationId { get; init; }

    public required NickName NickName  { get; set; }

    public required bool IsAccepted { get; init; }

    #region RelationShips

    public Conversation? Conversation { get; init; }

    public List<Message>? MessageList { get; init; }

    #endregion
}