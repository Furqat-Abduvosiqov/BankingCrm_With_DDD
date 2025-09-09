using Application.Shared.Interfaces.Specifications;
using FluentAssertions;
using Infrastructure.Shared.Specifications;

namespace Infrastructure.Shared.Tests.Specifications;

public class SpecificationTests : TestDataGenerator
{
    private readonly List<Customer> _customers = new()
    {
        new Customer { Id = 1, Name = "Alice", Age = 25 },
        new Customer { Id = 2, Name = "Bob", Age = 30 },
        new Customer { Id = 3, Name = "Charlie", Age = 20 },
        new Customer { Id = 4, Name = "Frank", Age = 28 },
        new Customer { Id = 5, Name = "John", Age = 29 },
        new Customer { Id = 6, Name = "Barry", Age = 31 },
    };
    
    private readonly ISpecificationEvaluator<Customer> _evaluator = new SpecificationEvaluator<Customer>();

    [Fact]
    public void Criteria_Should_Filter_Customers_By_Name()
    {
        var spec = CustomerByNameSpecification("Alice");
        var query = _evaluator.GetQuery(_customers.AsQueryable(), spec);

        query.AsEnumerable().Should().ContainSingle(c => c.Name == "Alice");
    }

    [Fact]
    public void ApplyOrderBy_Should_Order_By_Age()
    {
        var spec = OrderByAgeSpecification();
        var query = _evaluator.GetQuery(_customers.AsQueryable(), spec);

        query.Select(c => c.Age).AsEnumerable().Should().BeInAscendingOrder();
    }

    [Fact]
    public void ApplyOrderByDescending_Should_Order_By_Age()
    {
        var spec = OrderByAgeDescendingSpecification();
        var query = _evaluator.GetQuery(_customers.AsQueryable(), spec);

        query.Select(c => c.Age).AsEnumerable().Should().BeInDescendingOrder();
    }

    [Fact]
    public void ApplyOffsetPagination_Should_Skip_And_Take()
    {
        var spec = OffsetPaginationSpecification(1, 1);
        var query = _evaluator.GetQuery(_customers.AsQueryable(), spec);

        query.AsEnumerable().Should().ContainSingle().Which.Name.Should().Be("Bob");
    }
    
    [Fact]
    public void CursorSpec_ShouldReturnItemsAfterCursor()
    {
        var spec = CursorPaginationSpecification(cursor: "5", take: 3);

        // Act
        var result = _evaluator.GetQuery(_customers.AsQueryable(), spec);

        // Assert
        result.AsEnumerable().Should().HaveCount(1);
        result.Select(x => x.Id).AsEnumerable().Should().ContainInOrder(6);
    }

    [Fact]
    public void AddInclude_Should_Add_To_Includes()
    {
        var spec = IncludeSpecification();

        spec.Includes.Should().ContainSingle();
    }

    [Fact]
    public void Multiple_Specifications_Should_Compose_Correctly()
    {
        var spec = Specification<Customer>
            .Create()
            .Take(2)
            .OrderByDescending(c => c.Age)
            .Build();

        var query = _evaluator.GetQuery(_customers.AsQueryable(), spec);

        var result = query.ToList();

        result.Should().HaveCount(2);
        result.First().Name.Should().Be("Barry"); // Age 31
        result.Last().Name.Should().Be("Bob");   // Age 30
    }
}