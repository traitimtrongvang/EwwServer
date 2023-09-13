using System.Diagnostics.CodeAnalysis;
using Application.Domain.Entities;
using Application.Domain.ValueObjects;
using Application.Driven.EwwDatabase.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EwwDatabase.Repositories;

public class ConversationRepository : BaseRepository<Conversation, ConversationId>, IConversationRepository
{
    [SetsRequiredMembers]
    public ConversationRepository(EwwDatabaseContext db) : base(db)
    {
    }

    public async Task<bool> IsCoupleExistAsync(HashSet<UserId> memberUserIdSet, CancellationToken cancellationToken = default)
    {
        if (memberUserIdSet.Count != 2)
            throw new NotImplementedException(); // TODO
        
        var matchConversation = await Source
            .AsNoTracking()
            .Where(c => c.Type == ConversationTypeEnum.Couple)
            .Include(c => c.MemberList!.Where(m => memberUserIdSet.Contains(m.UserId)))
            .SingleOrDefaultAsync(cancellationToken);
        
        return matchConversation is not null && matchConversation.MemberList!.Count == memberUserIdSet.Count;
    }

    // public async Task<bool> IsExistByIdAndMemberUserId(ConversationId id, UserId memberUserId, CancellationToken cancellationToken = default)
    // {
    //     var conversation = await Source
    //         .AsNoTracking()
    //         .Include(c => c.MemberList!.Where(m => m.UserId == memberUserId))
    //         .Where(c => c.Id == id)
    //         .SingleOrDefaultAsync(cancellationToken);
    //
    //     return conversation is not null && conversation.MemberList!.Count == 1;
    // }
}