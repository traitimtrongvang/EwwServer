using Application.Driven.EwwDatabase.Repositories;

namespace Application.Driven.EwwDatabase;

// I suggest only use it when you want Unit Of Work pattern
public interface IEwwDatabaseContext
{
    /// <summary>
    /// I suggest only use it when you want Unit Of Work, on other hand, use <see cref="IBaseRepository{TEntity,TId}.InsertAsync"/>
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}