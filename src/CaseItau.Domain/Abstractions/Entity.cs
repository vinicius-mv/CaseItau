namespace CaseItau.Domain.Abstractions;

public abstract class Entity<TId>
{
    public virtual TId Id { get; private set; }

    protected Entity(TId id)
    {
        Id = id;
    }

    // Rehydration
    protected Entity() 
    { 
    }
}
