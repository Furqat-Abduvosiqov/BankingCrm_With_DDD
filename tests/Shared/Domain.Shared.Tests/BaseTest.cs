using Domain.Shared.ValueObjects;

namespace Domain.Shared.Tests;

/// <summary>
/// Base test class providing concrete implementations of classes and some general settings.
/// </summary>
public abstract class BaseTest
{
    /// <summary>
    /// Concrete implementation of EntityId for testing purposes.
    /// </summary>
    public sealed class OrderId : EntityId<OrderId>
    {
        public OrderId(Guid value) : base(value) { }
    }
    
    /// <summary>
    /// Concrete implementation of ValueObject for testing purposes.
    /// </summary>
    public sealed class Money : ValueObject
    {
        public decimal Amount { get; }
        public string Currency { get; }

        public Money(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }
    }
}

