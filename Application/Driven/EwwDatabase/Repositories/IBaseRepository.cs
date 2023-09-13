using Application.Domain.Entities;
using Application.Domain.ValueObjects;

namespace Application.Driven.EwwDatabase.Repositories;

public interface IBaseRepository<TEntity, in TId> 
    where TEntity : BaseEntity<TId> 
    where TId : BaseGuidId
{
    /// <summary>
    /// use this function to directly save to database, other repository can implement it to save to cache
    /// </summary>
    /// <waring>
    /// should not use this function when you want to use <see cref="IEwwDatabaseContext.SaveChangesAsync"/> or <see cref="TrackAsync"/>
    /// </waring>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// add to local to track, only save to database when calling <see cref="IEwwDatabaseContext.SaveChangesAsync"/>
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task TrackAsync(TEntity entity, CancellationToken cancellationToken = default);
    
    Task TrackManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// check if entity exist in db
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> IsExistByIdAsync(TId id, CancellationToken cancellationToken = default);
}