using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Shared.Specifications;

/// <summary>
/// Provides functionality to evaluate and apply specifications to an <see cref="IQueryable{T}"/> sequence,
/// including filtering, sorting, paging, and eager loading of related entities.
/// </summary>
public static class SpecificationEvaluator<T> where T : class
{
    public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
    {
        var query = inputQuery;

        if (specification.Criteria != null)
            query = query.Where(specification.Criteria);
        
        if (specification.CursorSelector != null && !string.IsNullOrEmpty(specification.Cursor))
        {
            var param = specification.CursorSelector.Parameters.First();
            var body = specification.CursorSelector.Body;

            // Handle boxing of value types (remove Convert expression if present)
            if (body.NodeType == ExpressionType.Convert && body is UnaryExpression unary)
                body = unary.Operand;

            // Parse cursor string into the property type
            var memberType = ((MemberExpression)body).Type;
            var typedValue = Convert.ChangeType(specification.Cursor, memberType);

            var constant = Expression.Constant(typedValue, memberType);
            var greaterThan = Expression.GreaterThan(body, constant);

            var lambda = Expression.Lambda<Func<T, bool>>(greaterThan, param);
            query = query.Where(lambda);
        }

        if (specification.OrderBy != null)
            query = query.OrderBy(specification.OrderBy);

        if (specification.OrderByDescending != null)
            query = query.OrderByDescending(specification.OrderByDescending);

        if (specification.Skip.HasValue)
            query = query.Skip(specification.Skip.Value);

        if (specification.Take.HasValue)
            query = query.Take(specification.Take.Value);

        query = specification.Includes
            .Aggregate(query, (current, include) => current.Include(include));

        return query;
    }
}
