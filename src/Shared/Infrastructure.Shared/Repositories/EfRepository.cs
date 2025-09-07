using System.Linq.Expressions;
using Domain.Shared.Entities;
using Domain.Shared.ValueObjects;
using Infrastructure.Shared.Paginations;
using Infrastructure.Shared.Repositories.Interfaces;
using Infrastructure.Shared.Specifications.Interfaces;
using Microsoft.EntityFrameworkCore;
using Utilities.Shared.Results;

namespace Infrastructure.Shared.Repositories;

public class EfRepository<T> : IRepository<T> where T : Entity
{
    private readonly DbContext _dbContext;
    private readonly IPaginationService<T> _paginationService;
    private readonly ISpecificationEvaluator<T> _specificationEvaluator;
    public EfRepository(DbContext dbContext, 
        IPaginationService<T> paginationService, 
        ISpecificationEvaluator<T> specificationEvaluator)
    {
        _dbContext = dbContext;
        _paginationService = paginationService;
        _specificationEvaluator = specificationEvaluator;
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
        _specificationEvaluator.GetQuery(_dbContext.Set<T>().AsQueryable(), spec);
    
    public async Task<PaginatedResult<T>> PaginateOffsetAsync(ISpecification<T> specification, int pageNumber, 
        int pageSize, CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(specification);
        return await _paginationService.PaginateOffsetAsync(query, pageNumber, pageSize, cancellationToken);
    }

    public async Task<PaginatedResult<T>> PaginateCursorAsync(ISpecification<T> specification, string? cursor, 
        int pageSize, CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(specification);
        
        if (specification.CursorSelector == null)
            throw new InvalidOperationException("CursorSelector must be set for cursor pagination.");
        
        return await _paginationService.PaginateCursorAsync(
            query,
            specification.CursorSelector,
            cursor,
            pageSize,
            cancellationToken);
    }
}
