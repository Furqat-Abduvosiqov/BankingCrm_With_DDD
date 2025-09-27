using Domain.Shared.Entities;

namespace Application.Shared.Interfaces.Repositories;

/// <summary>
/// Composite interface for convenience
/// </summary>
/// <typeparam name="TEntity">Represent Entity object</typeparam>
/// <typeparam name="TId">Represent Identification field of Entity</typeparam>
public interface IRepository<TEntity,TId> : IReadRepository<TEntity,TId>, IWriteRepository<TEntity>, IQueryRepository<TEntity>, IPaginatedRepository<TEntity>
    where TEntity : Entity
    where TId : notnull
{
}
