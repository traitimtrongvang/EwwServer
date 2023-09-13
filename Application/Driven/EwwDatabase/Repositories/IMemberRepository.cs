using Application.Domain.Entities;
using Application.Domain.ValueObjects;

namespace Application.Driven.EwwDatabase.Repositories;

public interface IMemberRepository : IBaseRepository<Member, MemberId>
{
    // TODO docs for this shit
    Task<Member?> FindByUserIdAndConversationIdAsync(UserId userId, ConversationId conversationId, CancellationToken cancellationToken = default);
}