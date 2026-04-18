namespace CaseItau.Domain.Abstractions
{
    public abstract class Entity<T> : IEquatable<Entity<T>>
    {
        protected Entity(T id)
        {
            Id = id;
        }

        // for rehydration
        protected Entity() 
        {
        }

        public T Id { get; init; }

        public bool Equals(Entity<T>? other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other is null) return false;

            return Id?.Equals(other.Id) ?? false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
