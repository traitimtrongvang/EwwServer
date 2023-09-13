using Auth0.Settings;

namespace StartUp.Configurations;

public class ExternalServiceConfigurationProvider : ConfigurationProvider
{
    public required string AuthIssuerSigningKeysUrl { get; init; }
    
    public override void Load()
    {
        Data = GetAuthConfigurations();
    }

    private IDictionary<string, string?> GetAuthConfigurations()
    {
        using var httpClient = new HttpClient();

        var issuerSigningKeysStr = httpClient.GetStringAsync(AuthIssuerSigningKeysUrl).Result;

        return new Dictionary<string, string?>()
        {
            [$"{nameof(Auth0Setting)}:{nameof(Auth0Setting.IssuerSigningKeysStr)}"] = issuerSigningKeysStr
        };
    }
}

public class ExternalServiceConfigurationSource : IConfigurationSource
{
    public required string AuthIssuerSigningKeysUrl { get; init; }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new ExternalServiceConfigurationProvider
        {
            AuthIssuerSigningKeysUrl = AuthIssuerSigningKeysUrl
        };
    }
}