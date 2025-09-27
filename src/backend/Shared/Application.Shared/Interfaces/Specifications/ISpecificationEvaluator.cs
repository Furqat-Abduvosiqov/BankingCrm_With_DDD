namespace Application.Shared.Interfaces.Specifications;

public interface ISpecificationEvaluator<T> where T : class
{
    IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification);
}