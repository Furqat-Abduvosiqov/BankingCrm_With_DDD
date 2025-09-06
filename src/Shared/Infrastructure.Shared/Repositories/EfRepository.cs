using System.Linq.Expressions;
using Domain.Shared.Entities;
using Domain.Shared.ValueObjects;
using Infrastructure.Shared.Repositories.Interfaces;
using Infrastructure.Shared.Specifications;
using Microsoft.EntityFrameworkCore;
using Utilities.Shared.Results;

namespace Infrastructure.Shared.Repositories;

public class EfRepository<T> : IRepository<T> where T : Entity
{
    private readonly DbContext _dbContext;

    public EfRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<T?> GetByIdAsync(EntityId id, CancellationToken cancellationToken = default) =>
        await _dbContext.Set<T>().FindAsync(new object[] { id }, cancellationToken);

    public async Task<IReadOnlyList<T>> ListAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.Set<T>().ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default) =>
        await ApplySpecification(specification).ToListAsync(cancellationToken);

    public async Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default) =>
        await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<T>().AddAsync(entity, cancellationToken);
        return entity;
    }

    public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<T>().Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<T>().Remove(entity);
        return Task.CompletedTask;
    }

    public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        => _dbContext.Set<T>().AnyAsync(predicate, cancellationToken);
    

    public Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
        => predicate is null 
        ? _dbContext.Set<T>().CountAsync(cancellationToken) 
        : _dbContext.Set<T>().CountAsync(predicate, cancellationToken);

    private IQueryable<T> ApplySpecification(ISpecification<T> spec) =>
        SpecificationEvaluator<T>.GetQuery(_dbContext.Set<T>().AsQueryable(), spec);
    
    public async Task<PaginatedResult<T>> PaginateOffsetAsync(ISpecification<T> specification, int pageNumber, 
        int pageSize, CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(specification);
        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return PaginatedResult<T>.CreateOffset(items, pageNumber, pageSize, totalCount);
    }

    public async Task<PaginatedResult<T>> PaginateCursorAsync(ISpecification<T> specification, string? cursor, 
        int pageSize, CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(specification);
        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query.Take(pageSize).ToListAsync(cancellationToken);

        var prevCursor = items.FirstOrDefault() is { } first ? first.GetHashCode().ToString() : null;
        var nextCursor = items.LastOrDefault() is { } last ? last.GetHashCode().ToString() : null;

        return PaginatedResult<T>.CreateCursor(items, pageSize, totalCount, prevCursor, nextCursor);
    }
}
