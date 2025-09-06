using Utilities.Shared.Paginations;

namespace Utilities.Shared.Results;

public sealed class PaginatedResult<T>
{
    public IReadOnlyList<T> Items { get; }
    public PaginationMetadata Metadata { get; }

    private PaginatedResult(IReadOnlyList<T> items, PaginationMetadata metadata)
    {
        Items = items;
        Metadata = metadata;
    }

    public static PaginatedResult<T> CreateOffset(
        IReadOnlyList<T> items,
        int pageNumber,
        int pageSize,
        int totalCount)
    {
        var metadata = new OffsetPaginationMetadata(pageNumber, pageSize, totalCount);
        return new PaginatedResult<T>(items, metadata);
    }

    public static PaginatedResult<T> CreateCursor(
        IReadOnlyList<T> items,
        int pageSize,
        int totalCount,
        string? previousCursor,
        string? nextCursor)
    {
        var metadata = new CursorPaginationMetadata(pageSize, totalCount, previousCursor, nextCursor);
        return new PaginatedResult<T>(items, metadata);
    }
}