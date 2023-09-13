using System.Diagnostics.CodeAnalysis;
using Application.Domain.Entities;
using Application.Domain.ValueObjects;
using Application.Driven.EwwDatabase.Repositories;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace EwwDatabase.Repositories;

public class MemberRepository : BaseRepository<Member, MemberId>, IMemberRepository
{
    // TODO wrong implementation, but I'm too lazy, that's just an example project
    private static readonly string CacheKey = "eww-database-member-hash";
    private readonly IDatabase _cacheDatabase;

    [SetsRequiredMembers]
    public MemberRepository(EwwDatabaseContext db, IConnectionMultiplexer connectionMultiplexer) : base(db)
    {
        _cacheDatabase = connectionMultiplexer.GetDatabase();
    }

    // TODO cache this shit
    public async Task<Member?> FindByUserIdAndConversationIdAsync(UserId userId, ConversationId conversationId, CancellationToken cancellationToken)
    {
        var memberKey = userId.Val + conversationId.Val;
        var memberVal = await _cacheDatabase.HashGetAsync(CacheKey, memberKey);

        Member? member = null;
        if (memberVal.IsNullOrEmpty)
        {
            member = await Source
                .Where(m => m.UserId == userId && m.ConversationId == conversationId)
                .SingleOrDefaultAsync(cancellationToken);
            await _cacheDatabase.HashSetAsync(CacheKey,memberKey, JsonConvert.SerializeObject(member));
        }
        else
        {
            member = JsonConvert.DeserializeObject<Member?>(memberVal!);
        }

        return member;
    }
}