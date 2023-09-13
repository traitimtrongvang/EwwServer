using Application.Domain.Entities;
using Application.Driven.EwwDatabase;
using Application.Driven.EwwDatabase.Repositories;
using EwwDatabase.Repositories;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace EwwDatabase.BackgroundServices;

// TODO this is a wrong implementation, think about load balanced environment and race conditional
public class MessageWriteBehind : BackgroundService
{
    private readonly PeriodicTimer _timer = new(TimeSpan.FromMinutes(30));
    private readonly IDatabase _cacheDatabase;
    private readonly IMessageRepository _messageRepository;
    private readonly IEwwDatabaseContext _ewwDatabaseUow;

    public MessageWriteBehind(IConnectionMultiplexer connectionMultiplexer, IEwwDatabaseContext ewwDatabaseUow, IMessageRepository messageRepository)
    {
        _ewwDatabaseUow = ewwDatabaseUow;
        _messageRepository = messageRepository;
        _cacheDatabase = connectionMultiplexer.GetDatabase();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await _timer.WaitForNextTickAsync(stoppingToken)
               && !stoppingToken.IsCancellationRequested)
        {
            await DoWorkAsync(stoppingToken);
        }
    }

    private async Task DoWorkAsync(CancellationToken cancellationToken = default)
    {
        var messageRedisValSet= await _cacheDatabase.SetMembersAsync(MessageRepository.CacheKey);
        var messageList = messageRedisValSet.Select(redisVal => JsonConvert.DeserializeObject<Message>(redisVal!)!);

        await _messageRepository.TrackManyAsync(messageList, cancellationToken);
        await _ewwDatabaseUow.SaveChangesAsync(cancellationToken);

        await _cacheDatabase.KeyDeleteAsync(MessageRepository.CacheKey);
    }
}