namespace CaseItau.Domain.Abstractions
{
    public abstract class Entity : IEquatable<Entity>
    {
        protected Entity(Guid id)
        {
            Id = id;
        }

        // for rehydration
        protected Entity() 
        {
        }

        public Guid Id { get; init; }

        public bool Equals(Entity? other)
        {
            if (other is null) return false;

            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
