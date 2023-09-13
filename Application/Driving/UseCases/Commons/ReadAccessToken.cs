using Application.Driven.AuthService;
using Application.Driven.AuthService.Dtos;

namespace Application.Driving.UseCases.Commons;

public record ReadAccessTokenReq
{
    public required string AccessToken { get; init; }
}

public record ReadAccessTokenRes : AccessTokenPayloadRes;

public interface IReadAccessToken
{
    ReadAccessTokenRes? Handle(ReadAccessTokenReq req);
}

public record ReadAccessToken : IReadAccessToken
{
    private readonly IAuthService _authService;

    public ReadAccessToken(IAuthService authService)
    {
        _authService = authService;
    }

    public ReadAccessTokenRes? Handle(ReadAccessTokenReq req)
    {
        var (_, accessTokenPayload) = _authService.Authorize(req.AccessToken);
        if (accessTokenPayload is null)
            return null;

        return new ReadAccessTokenRes
        {
            UserId = accessTokenPayload.UserId
        };
    }
}