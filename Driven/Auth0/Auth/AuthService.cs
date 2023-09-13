using System.IdentityModel.Tokens.Jwt;
using Application.Driven.AuthService;
using Application.Driven.AuthService.Dtos;
using Auth0.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Auth0.Auth;

public record AuthService : IAuthService
{
    private static readonly JwtSecurityTokenHandler TokenHandler = new();

    private readonly TokenValidationParameters _tokenValidationParameters;
    
    public AuthService(IOptions<Auth0Setting> options)
    {
        var auth0Setting = options.Value;
        _tokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = auth0Setting.Issuer,
            ValidAudiences = auth0Setting.Audiences,
            ValidateLifetime = false, // TODO - for development only
            IssuerSigningKeys = new JsonWebKeySet(auth0Setting.IssuerSigningKeysStr).GetSigningKeys(),
            RequireSignedTokens = false
        };
    }
    
    public (bool isAuthorize, AccessTokenPayloadRes?) Authorize(string accessToken)
    {
        try
        {
            var jwtToken = TokenHandler.ReadJwtToken(accessToken);
            
            TokenHandler.ValidateToken(accessToken, _tokenValidationParameters, out _);
            
            return (true, new AccessTokenPayloadRes
            {
                UserId = jwtToken.Subject
            });
        }
        catch
        {
            return (false, null);
        }
    }
}