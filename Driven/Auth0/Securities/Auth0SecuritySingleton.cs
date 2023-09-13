using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Auth0.Settings;
using Microsoft.Extensions.Options;

namespace Auth0.Securities;

public interface IAuth0SecuritySingleton
{
    public string AccessToken { get; }

    Task RefreshTokenAsync(CancellationToken cancellationToken = default);
}

public record Auth0SecuritySingleton : IAuth0SecuritySingleton
{
    private readonly IAuthenticationApiClient _authenticationApiClient;
    private readonly ClientCredentialsTokenRequest _clientCredentials;

    public string AccessToken { get; private set; } = null!;
    
    public Auth0SecuritySingleton(IOptions<Auth0Setting> options)
    {
        var auth0Setting = options.Value;
        
        _authenticationApiClient = new AuthenticationApiClient(new Uri(auth0Setting.DomainUrl));
        _clientCredentials = new ClientCredentialsTokenRequest
        {
            ClientId = auth0Setting.ClientId,
            ClientSecret = auth0Setting.ClientSecret,
            Audience = auth0Setting.DomainUrl + "/" + auth0Setting.ApiManagementUri + "/",
        };
    }
    
    public async Task RefreshTokenAsync(CancellationToken cancellationToken = default)
    {
        var response = await _authenticationApiClient.GetTokenAsync(_clientCredentials, cancellationToken);
        AccessToken = response.AccessToken;
    }
}