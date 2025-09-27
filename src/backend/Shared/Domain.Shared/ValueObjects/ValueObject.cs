using System.Diagnostics.CodeAnalysis;

namespace Domain.Shared.ValueObjects;

/// <summary>
/// Base class for Value Objects in Domain-Driven Design (DDD).
/// Provides value-based equality and enforces immutability.
/// </summary>
public abstract class ValueObject
{
    /// <summary>
    /// Must return all components that participate in equality comparison.
    /// Example: yield return Property1; yield return Property2;
    /// </summary>
    protected abstract IEnumerable<object> GetEqualityComponents();

    /// <summary>
    /// Equality operator uses value-based comparison.
    /// </summary>
    public static bool operator ==(ValueObject left, ValueObject right) => EqualOperator(left, right);

    public static bool operator !=(ValueObject left, ValueObject right) => NotEqualOperator(left, right);

    protected static bool EqualOperator(ValueObject? left, ValueObject? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;

        return left.Equals(right);
    }

    protected static bool NotEqualOperator(ValueObject left, ValueObject right) =>
        !EqualOperator(left, right);

    /// <summary>
    /// Value-based equality comparison.
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (obj is null || obj.GetType() != GetType()) return false;

        var other = (ValueObject)obj;

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    /// <summary>
    /// Hash code is based on all equality components.
    /// Uses HashCode.Combine for better distribution.
    /// </summary>
    [SuppressMessage("ReSharper", "ConditionalAccessQualifierIsNonNullableAccordingToAPIContract")]
    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var component in GetEqualityComponents())
            hash.Add(component);
        return hash.ToHashCode();
    }

    /// <summary>
    /// Creates a shallow copy of the ValueObject.
    /// Value Objects should be immutable, so this is rarely needed.
    /// </summary>
    public ValueObject ShallowCopy() =>
        MemberwiseClone() as ValueObject ?? throw new InvalidOperationException();
}
    