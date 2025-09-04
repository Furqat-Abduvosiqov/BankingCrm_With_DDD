using System.Diagnostics.CodeAnalysis;
using Domain.Shared.Events;

namespace Domain.Shared.Entities;

/// <summary>
/// Base class for all DDD entities.
/// Provides identity-based equality and domain event support.
/// </summary>
public abstract class Entity 
{
    public virtual Guid Id { get; protected set; }

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    #region Domain Events
    public void AddDomainEvent(IDomainEvent eventItem)
    {
        if (eventItem is null) throw new ArgumentNullException(nameof(eventItem));
        _domainEvents.Add(eventItem);
    }

    public void RemoveDomainEvent(IDomainEvent eventItem)
    {
        if (eventItem is null) throw new ArgumentNullException(nameof(eventItem));
        _domainEvents.Remove(eventItem);
    }

    public void ClearDomainEvents() => _domainEvents.Clear();
    #endregion

    #region Equality
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (obj is null || obj.GetType() != GetType()) return false;

        var other = (Entity)obj;

        if (Id.Equals(default) || other.Id.Equals(default))
            return false;

        return Id.Equals(other.Id);
    }

    [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
    public override int GetHashCode() =>
        HashCode.Combine(GetRealType(), Id);

    private Type GetRealType() =>
        GetType().IsGenericType ? GetType().BaseType! : GetType();

    public static bool operator ==(Entity? left, Entity? right) =>
        Equals(left, right);

    public static bool operator !=(Entity? left, Entity? right) =>
        !Equals(left, right);
    #endregion
}
