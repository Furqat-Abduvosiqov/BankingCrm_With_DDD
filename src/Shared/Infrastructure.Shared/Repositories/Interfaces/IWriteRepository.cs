using Domain.Shared.Entities;

namespace Infrastructure.Shared.Repositories.Interfaces;

/// <summary>
/// Represent interface for write operations
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IWriteRepository<T> where T : Entity
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
}