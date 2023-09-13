using Application.Driven.UserStorage;
using Application.Driven.UserStorage.Dtos;
using Auth0.Core.Exceptions;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Auth0.Securities;
using Auth0.Settings;
using Microsoft.Extensions.Options;

namespace Auth0.UserStorage;

public record UserStorageImpl : IUserStorage
{
    private readonly IAuth0SecuritySingleton _auth0Security;
    private readonly IManagementApiClient _managementApiClient;

    public UserStorageImpl(IAuth0SecuritySingleton auth0Security, IOptions<Auth0Setting> options)
    {
        var auth0Setting = options.Value;
        _auth0Security = auth0Security;
        _managementApiClient = new ManagementApiClient(_auth0Security.AccessToken, new Uri(auth0Setting.DomainUrl + "/" + auth0Setting.ApiManagementUri));
    }

    private async Task RefreshTokenAsync(CancellationToken cancellationToken = default)
    {
        await _auth0Security.RefreshTokenAsync(cancellationToken);
        _managementApiClient.UpdateAccessToken(_auth0Security.AccessToken);
    }

    public async Task<IDictionary<string, UserRes>> FindManyByIdSetAsync(HashSet<string> idSet, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = string.Join(" or ", idSet.Select(id => $"user_id={id}"));
            var getUserReq = new GetUsersRequest
            {
                Query = query
            };
            
            var userList = await _managementApiClient.Users.GetAllAsync(getUserReq, cancellationToken: cancellationToken);

            return userList.Select(u => new UserRes
            {
                Id = u.UserId,
                Name = u.NickName
            }).ToDictionary(u => u.Id);
        }
        catch (ErrorApiException)
        {
            await RefreshTokenAsync(cancellationToken);
            return await FindManyByIdSetAsync(idSet, cancellationToken);
        }
    }
}