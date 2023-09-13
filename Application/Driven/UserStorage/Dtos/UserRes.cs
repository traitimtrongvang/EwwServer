namespace Application.Driven.UserStorage.Dtos;

public record UserRes
{
    public required string Id { get; init; }

    public required string Name { get; init; }
}