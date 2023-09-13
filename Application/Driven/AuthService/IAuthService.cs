using Application.Driven.AuthService.Dtos;

namespace Application.Driven.AuthService;

public interface IAuthService
{
    // TODO docs for this shit
    (bool isAuthorize, AccessTokenPayloadRes?) Authorize(string accessToken);
}