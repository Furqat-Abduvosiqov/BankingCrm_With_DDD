using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Application.Shared.Interfaces.Paginations;
using Domain.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using Utilities.Shared.Results;

namespace Infrastructure.Shared.Paginations;

[SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract")]
public class PaginationService<T> : IPaginationService<T> where T : Entity
{
    public async Task<PaginatedResult<T>> PaginateOffsetAsync(
        IQueryable<T> query,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PaginatedResult<T>.CreateOffset(items, pageNumber, pageSize, totalCount);
    }

    public async Task<PaginatedResult<T>> PaginateCursorAsync(
        IQueryable<T> query,
        Expression<Func<T, object>> cursorSelector,
        string? cursor,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        // Apply cursor filter if provided
        if (!string.IsNullOrEmpty(cursor) && cursorSelector is not null)
        {
            query = ApplyCursorFilter(query, cursorSelector, cursor);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.Take(pageSize).ToListAsync(cancellationToken);

        // Use a stable, meaningful cursor (e.g., last item's ID or timestamp)
        var nextCursor = items.LastOrDefault() is { } last
            ? GetCursorValue(cursorSelector, last)?.ToString()
            : null;

        var prevCursor = items.FirstOrDefault() is { } first
            ? GetCursorValue(cursorSelector, first)?.ToString()
            : null;

        return PaginatedResult<T>.CreateCursor(items, pageSize, totalCount, prevCursor, nextCursor);
    }

    private static IQueryable<T> ApplyCursorFilter<T>(
        IQueryable<T> query,
        Expression<Func<T, object>> cursorSelector,
        string cursor)
    {
        var param = cursorSelector.Parameters.First();
        var body = cursorSelector.Body;

        // Handle boxing (Convert expression)
        if (body is UnaryExpression { NodeType: ExpressionType.Convert } unary)
            body = unary.Operand;

        if (body is not MemberExpression memberExpr)
            throw new ArgumentException("Cursor selector must be a member expression.");

        var memberType = memberExpr.Type;
        var typedValue = Convert.ChangeType(cursor, memberType);
        var constant = Expression.Constant(typedValue, memberType);
        var greaterThan = Expression.GreaterThan(body, constant);
        var lambda = Expression.Lambda<Func<T, bool>>(greaterThan, param);

        return query.Where(lambda);
    }

    private static object? GetCursorValue<T>(
        Expression<Func<T, object>>? cursorSelector,
        T entity)
    {
        if(cursorSelector is null) return entity;
        var compiled = cursorSelector.Compile();
        return compiled(entity);
    }
}