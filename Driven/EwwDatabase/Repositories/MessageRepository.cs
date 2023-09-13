using System.Diagnostics.CodeAnalysis;
using Application.Domain.Entities;
using Application.Domain.ValueObjects;
using Application.Driven.EwwDatabase.Repositories;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace EwwDatabase.Repositories;

public class MessageRepository : BaseRepository<Message, MessageId>, IMessageRepository
{
    // TODO wrong implementation, but I'm too lazy, that's just an example project
    public static readonly string CacheKey = "eww-database-message-set";
    private readonly IDatabase _cacheDatabase;
    
    [SetsRequiredMembers]
    public MessageRepository(EwwDatabaseContext db, IConnectionMultiplexer connectionMultiplexer) : base(db)
    {
        _cacheDatabase = connectionMultiplexer.GetDatabase();
    }

    /// <summary>
    /// Insert to cache, there is a worker service behind write data to main database
    /// </summary>
    public override async Task InsertAsync(Message entity, CancellationToken cancellationToken = default)
    {
        await _cacheDatabase.SetAddAsync(CacheKey, JsonConvert.SerializeObject(entity));
    }
}