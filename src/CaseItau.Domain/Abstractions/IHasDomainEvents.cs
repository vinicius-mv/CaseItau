namespace CaseItau.Domain.Abstractions;

/// <summary>
/// Marker interface to indicate that an entity has domain events.
/// </summary>
public interface IHasDomainEvents
{
    IReadOnlyList<IDomainEvent> GetDomainEvents();
    void ClearDomainEvents();
}
