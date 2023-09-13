namespace Application.Driven.AuthService.Dtos;

public record AccessTokenPayloadRes
{
    public required string UserId { get; init; }
}