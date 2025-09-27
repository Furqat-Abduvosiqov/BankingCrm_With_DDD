namespace Utilities.Shared.Paginations;

public abstract class PaginationMetadata
{
    public int PageSize { get; init; }
    public int TotalCount { get; init; }

    protected PaginationMetadata(int pageSize, int totalCount)
    {
        PageSize = pageSize;
        TotalCount = totalCount;
    }
}