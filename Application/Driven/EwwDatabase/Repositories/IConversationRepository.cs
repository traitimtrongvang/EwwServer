using Application.Domain.Entities;
using Application.Domain.ValueObjects;

namespace Application.Driven.EwwDatabase.Repositories;

public interface IConversationRepository : IBaseRepository<Conversation, ConversationId>
{
    Task<bool> IsCoupleExistAsync(HashSet<UserId> memberUserIdSet, CancellationToken cancellationToken = default);
    
    // Task<bool> IsExistByIdAndMemberUserId(ConversationId id, UserId memberUserId, CancellationToken cancellationToken = default);
}