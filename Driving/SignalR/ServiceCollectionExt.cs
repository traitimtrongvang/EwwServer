using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using SignalR.Providers;

namespace SignalR;

public static class ServiceCollectionExt
{
    public static void AddSignalRServices(this IServiceCollection services)
    {
        services.AddSignalR();

        services.AddSingleton<IUserIdProvider, UserIdProvider>();
    }
}