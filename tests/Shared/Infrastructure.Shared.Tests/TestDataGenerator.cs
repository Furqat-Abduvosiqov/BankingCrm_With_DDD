using Infrastructure.Shared.Repositories;
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
    
    /// <summary>
    ///  A spec with criteria
    /// </summary>
    public class CustomerByNameSpecification : BaseSpecification<Customer>
    {
        public CustomerByNameSpecification(string name)
            : base(c => c.Name == name)
        {
        }
    }

    /// <summary>
    /// A spec with ordering
    /// </summary>
    public class OrderByAgeSpecification : BaseSpecification<Customer>
    {
        public OrderByAgeSpecification()
        {
            ApplyOrderBy(c => c.Age);
        }
    }

    /// <summary>
    ///  A spec with descending ordering
    /// </summary>
    public class OrderByAgeDescendingSpecification : BaseSpecification<Customer>
    {
        public OrderByAgeDescendingSpecification()
        {
            ApplyOrderByDescending(c => c.Age);
        }
    }

    /// <summary>
    /// A spec with paging
    /// </summary>
    public class PagingSpecification : BaseSpecification<Customer>
    {
        public PagingSpecification(int skip, int take)
        {
            ApplyOffsetPagination(skip, take);
        }
    }
    
    /// <summary>
    /// A spec with include (fake include just to check it is stored)
    /// </summary>
    public class IncludeSpecification : BaseSpecification<Customer>
    {
        public IncludeSpecification()
        {
            AddInclude(c => c.Name); // Normally would be a navigation property
        }
    }
}