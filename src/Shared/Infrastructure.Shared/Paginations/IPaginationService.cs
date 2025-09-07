using System.Linq.Expressions;
using Domain.Shared.Entities;
using Utilities.Shared.Results;

namespace Infrastructure.Shared.Paginations;

public interface IPaginationService<T> where T : Entity
{
    Task<PaginatedResult<T>> PaginateOffsetAsync(
        IQueryable<T> query,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<PaginatedResult<T>> PaginateCursorAsync(
        IQueryable<T> query,
        Expression<Func<T, object>> cursorSelector,
        string? cursor,
        int pageSize,
        CancellationToken cancellationToken = default);
}