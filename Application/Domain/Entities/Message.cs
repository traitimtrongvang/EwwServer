using Application.Domain.ValueObjects;

namespace Application.Domain.Entities;

public class Message : BaseEntity<MessageId>
{
    public required MemberId SenderMemberId { get; init; }

    public required MessageContent Content { get; set; }

    #region RelationShips

    public Member? SenderMember { get; init; }

    #endregion
}