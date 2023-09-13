using Application.Driven.AuthService;
using Application.Driven.UserStorage;
using Auth0.Auth;
using Auth0.Securities;
using Auth0.Settings;
using Auth0.UserStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Auth0;

public static class ServiceCollectionExt
{
    public static void AddAuth0Services(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<Auth0Setting>(
            options => configuration.GetSection(nameof(Auth0Setting)).Bind(options));

        services.AddSingleton<IAuth0SecuritySingleton, Auth0SecuritySingleton>();
        services.AddScoped<IUserStorage, UserStorageImpl>();

        services.AddScoped<IAuthService, AuthService>();
    }
}