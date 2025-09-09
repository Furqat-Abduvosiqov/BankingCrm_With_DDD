using Domain.Shared.Entities;

namespace Application.Shared.Interfaces.Repositories;

/// <summary>
/// Composite interface for convenience
/// </summary>
/// <typeparam name="T">Represent Entity object</typeparam>
public interface IRepository<T> : IReadRepository<T>, IWriteRepository<T>, IQueryRepository<T>, IPaginatedRepository<T>
    where T : Entity
{
}
