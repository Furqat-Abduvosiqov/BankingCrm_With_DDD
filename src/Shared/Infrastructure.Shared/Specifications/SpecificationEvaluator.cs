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
