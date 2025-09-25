using Domain.Shared.Entities;
using Domain.Shared.Events;
using Domain.Shared.ValueObjects;
#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.

namespace Domain.Shared.Tests;

/// <summary>
/// Test Data generator class
/// </summary>
public abstract class TestDataGenerator
{
    /// <summary>
    /// Concrete implementation of EntityId for testing purposes.
    /// </summary>
    public class OrderId : EntityId<int>
    {
        public OrderId() { }

        public OrderId(int value) : base(value) { }

        public static OrderId New(int value) => Create<OrderId>(value);
    }

    public class TestId<TValue> : EntityId<TValue>
    {
        public TestId(TValue value) : base(value) { }
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
    
    /// <summary>
    /// Concrete implementation of Entity for testing purposes.
    /// </summary>
    public sealed class TestEntity : Entity
    {
        public TestEntity(Guid? id = null)
        {
            Id = id ?? Guid.NewGuid();
        }
    }
    
    /// <summary>
    /// Concrete implementation of IDomainEvent for testing purposes.
    /// </summary>
    protected class TestDomainEvent : IDomainEvent
    {
        public TestDomainEvent(DateTimeOffset occurredOn)
        {
            OccurredOn = occurredOn;
        }

        public DateTimeOffset OccurredOn { get; }
    }
}

