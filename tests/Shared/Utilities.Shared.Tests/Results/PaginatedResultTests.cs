using FluentAssertions;
using Utilities.Shared.Paginations;
using Utilities.Shared.Results;

namespace Utilities.Shared.Tests.Results;

public class PaginatedResultTests
{
    [Fact]
    public void CreateOffset_ShouldCalculateTotalPagesCorrectly()
    {
        // Arrange
        var items = new List<int> { 1, 2, 3, 4, 5 };
        var totalCount = 23;
        var pageSize = 5;
        var pageNumber = 3;

        // Act
        var result = PaginatedResult<int>.CreateOffset(items, pageNumber, pageSize, totalCount);

        // Assert
        result.Items.Should().HaveCount(5);
        result.Metadata.Should().BeOfType<OffsetPaginationMetadata>()
            .Which.Should().Match<OffsetPaginationMetadata>(m =>
                m.PageSize == 5 &&
                m.TotalPages == 5 &&
                m.PageNumber == 3 &&
                m.TotalCount == 23);
    }

    [Fact]
    public void CreateCursor_ShouldProducePreviousAndNextCursor()
    {
        // Arrange
        var items = new List<int> { 11, 12, 13, 14, 15 };
        string previousCursor = "10";
        string nextCursor = "16";

        // Act
        var result = PaginatedResult<int>.CreateCursor(items, pageSize: 5, totalCount:10, previousCursor, nextCursor);

        // Assert
        result.Metadata.Should().BeOfType<CursorPaginationMetadata>()
            .Which.Should().Match<CursorPaginationMetadata>(m =>
                m.PreviousCursor == previousCursor &&
                m.NextCursor == nextCursor &&
                m.PageSize == 5);
    }
}