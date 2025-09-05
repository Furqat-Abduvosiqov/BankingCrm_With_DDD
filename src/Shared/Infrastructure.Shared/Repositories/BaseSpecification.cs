using System.Linq.Expressions;
using Infrastructure.Shared.Repositories.Interfaces;

namespace Infrastructure.Shared.Repositories;

/// <summary>
/// Base class for defining query specifications, including filtering, sorting, and paging logic for entities of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The entity type the specification applies to.</typeparam>
public abstract class BaseSpecification<T> : ISpecification<T>
{
    protected BaseSpecification(Expression<Func<T, bool>>? criteria = null)
    {
        Criteria = criteria;
    }

    public Expression<Func<T, bool>>? Criteria { get; }
    public List<Expression<Func<T, object>>> Includes { get; } = new();
    public Expression<Func<T, object>>? OrderBy { get; private set; }
    public Expression<Func<T, object>>? OrderByDescending { get; private set; }
    public int? Take { get; private set; }
    public int? Skip { get; private set; }

    protected void AddInclude(Expression<Func<T, object>> include) => Includes.Add(include);
    
    protected void ApplyPaging(int skip, int take) { Skip = skip; Take = take; }
    
    protected void ApplyOrderBy(Expression<Func<T, object>> orderByExpression) => OrderBy = orderByExpression;
    
    protected void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression) => OrderByDescending = orderByDescendingExpression;
}
