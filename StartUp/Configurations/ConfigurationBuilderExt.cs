using Auth0.Settings;

namespace StartUp.Configurations;

public static class ConfigurationBuilderExt
{
    public static IConfiguration BuildCustomConfiguration(this IServiceCollection services)
    {
        #region decide environment
        
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        
        var configFilePath = $"appsettings.{environment}.json";
        
        #endregion

        #region build config

        var configurationBuilder = new ConfigurationBuilder();
        
        configurationBuilder
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile(configFilePath, optional: true, reloadOnChange: true)
            .AddExternalServiceConfiguration();

        var config = configurationBuilder.Build();

        #endregion
        
        return config;
    }

    private static IConfigurationBuilder AddExternalServiceConfiguration(this IConfigurationBuilder builder)
    {
        var tempConfig = builder.Build();
        
        var auth0Setting = tempConfig.GetSection(nameof(Auth0Setting)).Get<Auth0Setting>()!;

        return builder.Add(new ExternalServiceConfigurationSource
        {
            AuthIssuerSigningKeysUrl = $"{auth0Setting.DomainUrl}/{auth0Setting.PublicKeyUri}"
        });
    }
}