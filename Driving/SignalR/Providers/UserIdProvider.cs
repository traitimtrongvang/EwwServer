using Application.Driving.Exceptions;
using Application.Driving.UseCases.Commons;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace SignalR.Providers;

public class UserIdProvider : IUserIdProvider
{
    private readonly IReadAccessToken _readAccessToken;

    public UserIdProvider(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        _readAccessToken = scope.ServiceProvider.GetRequiredService<IReadAccessToken>();
    }
    
    public string? GetUserId(HubConnectionContext connection)
    {
        var accessToken = connection.GetHttpContext()?.Request.Query["access-token"].ToString() ?? "";
        
        var payload = _readAccessToken.Handle(new ()
        {
            AccessToken = accessToken
        });

        if (payload is null)
            throw new UnauthorizedExc();
        
        return payload.UserId;
    }
}