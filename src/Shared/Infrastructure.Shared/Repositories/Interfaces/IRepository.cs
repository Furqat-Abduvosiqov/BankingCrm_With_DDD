using System.Linq.Expressions;
using Domain.Shared.Entities;
using Domain.Shared.ValueObjects;
using Infrastructure.Shared.Specifications;
using Utilities.Shared.Results;

namespace Infrastructure.Shared.Repositories.Interfaces;

public interface IRepository<T> where T : Entity
{
    Task<T?> GetByIdAsync(EntityId id, CancellationToken cancellationToken = default);
    Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> ListAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
    
    Task<PaginatedResult<T>> PaginateOffsetAsync(ISpecification<T> specification, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<PaginatedResult<T>> PaginateCursorAsync(ISpecification<T> specification, string? cursor, int pageSize, CancellationToken cancellationToken = default);
    
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
}
