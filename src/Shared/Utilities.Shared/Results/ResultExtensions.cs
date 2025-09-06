namespace Utilities.Shared.Results;

public static class ResultExtensions
{
    public static Result<PaginatedResult<T>> SuccessPaginatedOffset<T>(
        IReadOnlyList<T> items,
        int pageNumber,
        int pageSize,
        int totalCount)
    {
        return Result<PaginatedResult<T>>.Success(
            PaginatedResult<T>.CreateOffset(items, pageNumber, pageSize, totalCount));
    }

    public static Result<PaginatedResult<T>> SuccessPaginatedCursor<T>(
        IReadOnlyList<T> items,
        int pageSize,
        int totalCount,
        string? previousCursor,
        string? nextCursor)
    {
        return Result<PaginatedResult<T>>.Success(
            PaginatedResult<T>.CreateCursor(items, pageSize, totalCount, previousCursor, nextCursor));
    }
}