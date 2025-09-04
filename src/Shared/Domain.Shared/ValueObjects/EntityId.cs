namespace Domain.Shared.ValueObjects;

/// <summary>
/// Strongly-typed identifier for entities.
/// Prevents primitive obsession with raw GUIDs.
/// </summary>
public abstract class EntityId : ValueObject
{
    public Guid Value { get; }

    protected EntityId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("ID cannot be empty.", nameof(value));

        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}

/// <summary>
/// Strongly-typed ID with factory method for creation.
/// Example: public sealed class OrderId : EntityId&lt;OrderId&gt; { private OrderId(Guid value) : base(value) {} }
/// </summary>
public abstract class EntityId<T> : EntityId where T : EntityId<T>
{
    protected EntityId(Guid value) : base(value) { }

    public static T New() => (T)Activator.CreateInstance(typeof(T), Guid.NewGuid())!;
    public static T FromGuid(Guid value) => (T)Activator.CreateInstance(typeof(T), value)!;
}
