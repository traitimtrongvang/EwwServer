using Application.Driven.UserStorage.Dtos;

namespace Application.Driven.UserStorage;

public interface IUserStorage
{
    // TODO docs for this shit
    Task<IDictionary<string, UserRes>> FindManyByIdSetAsync(HashSet<string> idSet, CancellationToken cancellationToken = default);
}