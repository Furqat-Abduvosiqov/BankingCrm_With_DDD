using System.Linq.Expressions;
using Application.Shared.Interfaces.Paginations;
using Application.Shared.Interfaces.Repositories;
using Application.Shared.Interfaces.Specifications;
using Domain.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using Utilities.Shared.Results;

namespace Infrastructure.Shared.Repositories;

public class EfRepository<TEntity,TId> : IRepository<TEntity,TId> 
    where TEntity : Entity
    where TId : notnull
{
    private readonly DbContext _dbContext;
    private readonly IPaginationService<TEntity> _paginationService;
    private readonly ISpecificationEvaluator<TEntity> _specificationEvaluator;
    public EfRepository(DbContext dbContext, 
        IPaginationService<TEntity> paginationService, 
        ISpecificationEvaluator<TEntity> specificationEvaluator)
    {
        _dbContext = dbContext;
        _paginationService = paginationService;
        _specificationEvaluator = specificationEvaluator;
    }

    public async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default) =>
        await _dbContext.Set<TEntity>().FindAsync(new object[] { id }, cancellationToken);

    public async Task<IReadOnlyList<TEntity>> ListAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.Set<TEntity>().ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default) =>
        await ApplySpecification(specification).ToListAsync(cancellationToken);

    public async Task<TEntity?> FirstOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default) =>
        await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<TEntity>().AddAsync(entity, cancellationToken);
        return entity;
    }

    public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<TEntity>().Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<TEntity>().Remove(entity);
        return Task.CompletedTask;
    }

    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => _dbContext.Set<TEntity>().AnyAsync(predicate, cancellationToken);
    

    public Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
        => predicate is null 
        ? _dbContext.Set<TEntity>().CountAsync(cancellationToken) 
        : _dbContext.Set<TEntity>().CountAsync(predicate, cancellationToken);

    private IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> spec) =>
        _specificationEvaluator.GetQuery(_dbContext.Set<TEntity>().AsQueryable(), spec);
    
    public async Task<PaginatedResult<TEntity>> PaginateOffsetAsync(ISpecification<TEntity> specification, int pageNumber, 
        int pageSize, CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(specification);
        return await _paginationService.PaginateOffsetAsync(query, pageNumber, pageSize, cancellationToken);
    }

    public async Task<PaginatedResult<TEntity>> PaginateCursorAsync(ISpecification<TEntity> specification, string? cursor, 
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
