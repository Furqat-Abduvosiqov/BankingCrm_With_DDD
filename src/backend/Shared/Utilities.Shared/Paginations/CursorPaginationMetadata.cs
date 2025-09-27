namespace Utilities.Shared.Paginations;

public class CursorPaginationMetadata : PaginationMetadata
{
    public string? PreviousCursor { get; init; }
    public string? NextCursor { get; init; }

    public CursorPaginationMetadata(int pageSize, int totalCount, string? previousCursor, string? nextCursor)
        : base(pageSize, totalCount)
    {
        PreviousCursor = previousCursor;
        NextCursor = nextCursor;
    }
}