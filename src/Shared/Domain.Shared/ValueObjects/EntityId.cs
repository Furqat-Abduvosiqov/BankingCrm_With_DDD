namespace Domain.Shared.ValueObjects;

/// <summary>
/// Strongly-typed identifier for entities.
/// Prevents primitive obsession with raw values.
/// </summary>
public abstract class EntityId<TValue> : ValueObject
    where TValue : notnull
{
    public TValue Value { get; private set; } = default!;
    
    protected EntityId() { }

    protected EntityId(TValue value)
    {
        SetValue(value);
    }
    
    protected void SetValue(TValue value)
    {
        if (IsDefault(value))
            throw new ArgumentException("ID cannot be default/empty.", nameof(value));

        if (!EqualityComparer<TValue>.Default.Equals(Value, default!))
            throw new InvalidOperationException("EntityId value can only be set once.");

        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString() ?? string.Empty;

    private static bool IsDefault(TValue value)
    {
        if (EqualityComparer<TValue>.Default.Equals(value, default!))
            return true;

        return value switch
        {
            string s when string.IsNullOrWhiteSpace(s) => true,
            Guid g when g == Guid.Empty => true,
           
            long and <= 0 => true,
            int and <= 0 => true,
            short and <= 0 => true,
            sbyte and <= 0 => true,
            
            uint and 0 => true,
            ushort and 0 => true,
            ulong and 0 => true,
            byte and 0 => true,

            _ => false
        };
    }
    
    public static TId Create<TId>(TValue value)
        where TId : EntityId<TValue>, new()
    {
        var id = new TId();
        id.SetValue(value);
        return id;
    }
}
