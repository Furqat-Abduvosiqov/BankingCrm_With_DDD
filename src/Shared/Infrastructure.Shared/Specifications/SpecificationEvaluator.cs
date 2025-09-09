using System.Linq.Expressions;
using Application.Shared.Interfaces.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Shared.Specifications;

/// <summary>
/// Provides functionality to evaluate and apply specifications to an <see cref="IQueryable{T}"/> sequence,
/// including filtering, sorting, paging, and eager loading of related entities.
/// </summary>
public class SpecificationEvaluator<T> : ISpecificationEvaluator<T> where T : class
{
    public virtual IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
    {
        var query = inputQuery;

        if (specification.Criteria != null)
            query = query.Where(specification.Criteria);

        if (specification.CursorSelector != null && !string.IsNullOrEmpty(specification.Cursor))
        {
            query = ApplyCursorFilter(query, specification.CursorSelector, specification.Cursor);
        }

        if (specification.OrderBy != null)
            query = query.OrderBy(specification.OrderBy);
        else if (specification.OrderByDescending != null)
            query = query.OrderByDescending(specification.OrderByDescending);

        if (specification.Skip.HasValue)
            query = query.Skip(specification.Skip.Value);
        if (specification.Take.HasValue)
            query = query.Take(specification.Take.Value);

        query = specification.Includes
            .Aggregate(query, (current, include) => current.Include(include));

        return query;
    }

    protected virtual IQueryable<T> ApplyCursorFilter(
        IQueryable<T> query,
        Expression<Func<T, object>> cursorSelector,
        string cursor)
    {
        var param = cursorSelector.Parameters.First();
        var body = cursorSelector.Body;

        if (body is UnaryExpression { NodeType: ExpressionType.Convert } unary)
            body = unary.Operand;

        if (body is not MemberExpression memberExpr)
            throw new ArgumentException("Cursor selector must target a member.");

        var memberType = memberExpr.Type;
        var typedValue = Convert.ChangeType(cursor, memberType);
        var constant = Expression.Constant(typedValue, memberType);
        var greaterThan = Expression.GreaterThan(body, constant);
        var lambda = Expression.Lambda<Func<T, bool>>(greaterThan, param);

        return query.Where(lambda);
    }
}
