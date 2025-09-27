using System.Linq.Expressions;
using Application.Shared.Interfaces.Specifications;


namespace Infrastructure.Shared.Specifications;

/// <summary>
/// Base class for defining query specifications, including filtering, sorting, and paging logic for entities of type <typeparamref name="T"/>.
/// </summary>
public class Specification<T> : ISpecification<T>
{
    public Expression<Func<T, bool>>? Criteria { get; }
    public List<Expression<Func<T, object>>> Includes { get; }
    public Expression<Func<T, object>>? OrderBy { get; private set; }
    public Expression<Func<T, object>>? OrderByDescending { get; private set; }
    public int? Take { get; private set; }
    public int? Skip { get; private set; }
    
    public string? Cursor { get; private set; }
    
    public Expression<Func<T, object>>? CursorSelector { get; private set; }
    
     private Specification(
        Expression<Func<T, bool>>? criteria = null,
        List<Expression<Func<T, object>>>? includes = null,
        Expression<Func<T, object>>? orderBy = null,
        Expression<Func<T, object>>? orderByDescending = null,
        int? take = null,
        int? skip = null,
        string? cursor = null,
        Expression<Func<T, object>>? cursorSelector = null)
    {
        Criteria = criteria;
        Includes = includes ?? new List<Expression<Func<T, object>>>();
        OrderBy = orderBy;
        OrderByDescending = orderByDescending;
        Take = take;
        Skip = skip;
        Cursor = cursor;
        CursorSelector = cursorSelector;
    }

    /// <summary>
    /// Creates a new specification builder for type <typeparamref name="T"/>.
    /// </summary>
    public static SpecificationBuilder<T> Create() => new();
    
    #region Specification Builder 
    public class SpecificationBuilder<TItem>
    {
        private Expression<Func<TItem, bool>>? _criteria;
        private readonly List<Expression<Func<TItem, object>>> _includes = new();
        private Expression<Func<TItem, object>>? _orderBy;
        private Expression<Func<TItem, object>>? _orderByDescending;
        private int? _take;
        private int? _skip;
        private string? _cursor;
        private Expression<Func<TItem, object>>? _cursorSelector;

        public SpecificationBuilder<TItem> Where(Expression<Func<TItem, bool>> criteria)
        {
            _criteria = criteria;
            return this;
        }

        public SpecificationBuilder<TItem> Include(Expression<Func<TItem, object>> include)
        {
            _includes.Add(include);
            return this;
        }

        public SpecificationBuilder<TItem> OrderBy(Expression<Func<TItem, object>> orderBy)
        {
            _orderBy = orderBy;
            return this;
        }

        public SpecificationBuilder<TItem> OrderByDescending(Expression<Func<TItem, object>> orderByDescending)
        {
            _orderByDescending = orderByDescending;
            return this;
        }

        public SpecificationBuilder<TItem> Take(int take)
        {
            _take = take;
            return this;
        }

        public SpecificationBuilder<TItem> Skip(int skip)
        {
            _skip = skip;
            return this;
        }

        public SpecificationBuilder<TItem> WithCursor(string? cursor, Expression<Func<TItem, object>> cursorSelector)
        {
            _cursor = cursor;
            _cursorSelector = cursorSelector;
            return this;
        }

        public ISpecification<TItem> Build()
        {
            if (!string.IsNullOrEmpty(_cursor) && _cursorSelector is null)
                throw new InvalidOperationException("CursorSelector is required when Cursor is specified.");

            if (_orderBy is not null && _orderByDescending is not null)
                throw new InvalidOperationException("Cannot specify both OrderBy and OrderByDescending.");
            
            return new Specification<TItem>(
                        _criteria,
                        _includes,
                        _orderBy,
                        _orderByDescending,
                        _take,
                        _skip,
                        _cursor,
                        _cursorSelector);
        } 
    }
    #endregion
}
