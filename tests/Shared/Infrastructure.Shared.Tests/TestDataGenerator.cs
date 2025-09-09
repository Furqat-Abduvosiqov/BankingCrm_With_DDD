using Application.Shared.Interfaces.Specifications;
using Infrastructure.Shared.Specifications;


namespace Infrastructure.Shared.Tests;

public abstract class TestDataGenerator
{
    /// <summary>
    /// Test entity for specification testing
    /// </summary>
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }
    
    public static ISpecification<Customer> CustomerByNameSpecification(string name) =>
        Specification<Customer>
            .Create()
            .Where(c => c.Name == name)
            .Build();

    public static ISpecification<Customer> OrderByAgeSpecification() =>
        Specification<Customer>
            .Create()
            .OrderBy(c => c.Age)
            .Build();

    public static ISpecification<Customer> OrderByAgeDescendingSpecification() =>
        Specification<Customer>
            .Create()
            .OrderByDescending(c => c.Age)
            .Build();

    public static ISpecification<Customer> OffsetPaginationSpecification(int skip, int take) =>
        Specification<Customer>
            .Create()
            .Skip(skip)
            .Take(take)
            .Build();

    public static ISpecification<Customer> CursorPaginationSpecification(string? cursor, int take) =>
        Specification<Customer>
            .Create()
            .OrderBy(x => x.Id)
            .WithCursor(cursor, x => x.Id)
            .Take(take)
            .Build();

    public static ISpecification<Customer> IncludeSpecification() =>
        Specification<Customer>
            .Create()
            .Include(c => c.Name)
            .Build();
}
