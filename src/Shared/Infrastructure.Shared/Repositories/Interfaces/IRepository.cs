using Domain.Shared.Entities;

namespace Infrastructure.Shared.Repositories.Interfaces;

/// <summary>
/// Composite interface for convenience
/// </summary>
/// <typeparam name="T">Represent Entity object</typeparam>
public interface IRepository<T> : IReadRepository<T>, IWriteRepository<T>, IQueryRepository<T>, IPaginatedRepository<T>
    where T : Entity
{
}
