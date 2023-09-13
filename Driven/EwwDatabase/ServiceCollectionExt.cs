using Application.Driven.EwwDatabase;
using Application.Driven.EwwDatabase.Repositories;
using EwwDatabase.BackgroundServices;
using EwwDatabase.Repositories;
using EwwDatabase.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace EwwDatabase;

public static class ServiceCollectionExt
{
    public static void AddEwwDatabaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EwwDatabaseSetting>(
            options => configuration.GetSection(nameof(EwwDatabaseSetting)).Bind(options));
        
        var databaseSetting = configuration.GetSection(nameof(EwwDatabaseSetting)).Get<EwwDatabaseSetting>()!;

        services.AddHostedService<MessageWriteBehind>();
        
        services.AddSingleton<IConnectionMultiplexer>(
            options => ConnectionMultiplexer.Connect(databaseSetting.CacheConnectionStr));
        
        services.AddDbContext<EwwDatabaseContext>(
            options => options
                .UseNpgsql(databaseSetting.ConnectionStr));
        
        services.AddScoped<IEwwDatabaseContext>(
            sp => sp.GetRequiredService<EwwDatabaseContext>());
        
        services.AddScoped<IConversationRepository>(options 
            => new ConversationRepository(options.GetRequiredService<EwwDatabaseContext>()));
        
        services.AddScoped<IMemberRepository>(options 
            => new MemberRepository(options.GetRequiredService<EwwDatabaseContext>(), options.GetRequiredService<IConnectionMultiplexer>()));
        
        services.AddScoped<IMessageRepository>(options 
            => new MessageRepository(options.GetRequiredService<EwwDatabaseContext>(), options.GetRequiredService<IConnectionMultiplexer>()));
    }
}