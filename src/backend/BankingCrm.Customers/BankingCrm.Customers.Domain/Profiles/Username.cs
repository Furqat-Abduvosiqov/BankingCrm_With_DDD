using Domain.Shared.ValueObjects;

namespace BankingCrm.Customers.Domain.Profiles;

public class Username : ValueObject
{
    public string Value { get; set; }

    public Username(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));
        
        Value = value;
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
