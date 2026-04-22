using CaseItau.Domain.Abstractions;

namespace CaseItau.Domain.UnitTests.Common;

public abstract class BaseTest
{
    public static T AssertDomainEventWasPublished<T>(IHasDomainEvents aggregateRoot)
        where T : IDomainEvent
    {
        var domainEvent = aggregateRoot.GetDomainEvents().OfType<T>().LastOrDefault();

        if (domainEvent is null)
            throw new Exception($"Expected domain event of type {typeof(T).Name} was not published.");

        return domainEvent;
    }
}
