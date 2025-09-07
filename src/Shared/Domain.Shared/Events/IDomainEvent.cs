namespace Domain.Shared.Events;

/// <summary>
/// Base abstraction for Domain Events
/// </summary>
public interface IDomainEvent
{
    public DateTimeOffset OccurredOn { get; }
}