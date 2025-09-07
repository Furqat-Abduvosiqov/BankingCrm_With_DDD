using System.Linq.Expressions;

namespace Infrastructure.Shared.Specifications.Interfaces;

public interface ISpecification<T>
{
    Expression<Func<T, bool>>? Criteria { get; }
    List<Expression<Func<T, object>>> Includes { get; }
    Expression<Func<T, object>>? OrderBy { get; }
    Expression<Func<T, object>>? OrderByDescending { get; }
    int? Take { get; }
    int? Skip { get; }
    
    string? Cursor { get; }
    Expression<Func<T, object>>? CursorSelector { get; }
}