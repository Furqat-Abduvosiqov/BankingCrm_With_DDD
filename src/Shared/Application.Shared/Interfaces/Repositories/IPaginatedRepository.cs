using Application.Shared.Interfaces.Specifications;
using Domain.Shared.Entities;
using Utilities.Shared.Results;

namespace Application.Shared.Interfaces.Repositories;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IPaginatedRepository<T> where T : Entity
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="specification"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<PaginatedResult<T>> PaginateOffsetAsync(ISpecification<T> specification, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="specification"></param>
    /// <param name="cursor"></param>
    /// <param name="pageSize"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<PaginatedResult<T>> PaginateCursorAsync(ISpecification<T> specification, string? cursor, int pageSize, CancellationToken cancellationToken = default);
}