using System.Diagnostics.CodeAnalysis;
using Application.Domain.Entities;
using Application.Domain.ValueObjects;
using Application.Driven.EwwDatabase;
using Application.Driven.EwwDatabase.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EwwDatabase.Repositories;

public abstract class BaseRepository<TEntity, TId> : IBaseRepository<TEntity, TId>
    where TEntity : BaseEntity<TId> 
    where TId : BaseGuidId 
{
    protected DbSet<TEntity> Source { get; }

    public required EwwDatabaseContext Db { get; init; }

    [SetsRequiredMembers]
    protected BaseRepository(EwwDatabaseContext db)
    {
        Db = db;
        Source = db.Set<TEntity>();
    }
    
    public virtual async Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await Source.AddAsync(entity, cancellationToken);
        await Db.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// add to local to track, only save to database when calling <see cref="IEwwDatabaseContext.SaveChangesAsync"/>
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task TrackAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await Source.AddAsync(entity, cancellationToken);
    }

    public async Task TrackManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await Source.AddRangeAsync(entities, cancellationToken);
    }
    
    public async Task<bool> IsExistByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        return await Source
            .AnyAsync(e => e.Id.Val == id.Val, cancellationToken);
    }
}