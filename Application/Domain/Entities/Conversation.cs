using Application.Domain.ValueObjects;

namespace Application.Domain.Entities;

public class Conversation : BaseEntity<ConversationId>
{
    public required UserId CreatorUserId { get; init; }

    public required ConversationTypeEnum Type { get; init; }

    public required ConversationName Name { get; set; }

    #region RelationShips

    public List<Member>? MemberList { get; init; }

    #endregion
}