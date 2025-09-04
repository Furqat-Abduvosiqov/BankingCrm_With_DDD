namespace Domain.Shared.Events;

/// <summary>
/// 
/// </summary>
public interface IDomainEvent
{
    public DateTimeOffset OccurredOn { get; }
}