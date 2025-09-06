namespace Utilities.Shared.Paginations;

public class OffsetPaginationMetadata : PaginationMetadata
{
    public int PageNumber { get; init; }
    
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    
    public OffsetPaginationMetadata(int pageNumber,int pageSize, int totalCount) : base(pageSize, totalCount)
    {
        PageNumber = pageNumber;
    }
}