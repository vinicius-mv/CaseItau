using CaseItau.Domain.Abstractions;
using CaseItau.Domain.Fundos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CaseItau.Infra;

public class ApplicationDbContext : DbContext, IUnitOfWork
{
    private readonly IPublisher _publisher;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IPublisher publisher) 
        : base(options)
    {
        _publisher = publisher;
    }

    public DbSet<Fundo> Fundos { get; private set; }

    public DbSet<TipoFundo> TipoFundos { get; private set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

        await PublishDomainEvents();

        return result;
    }

    private async Task PublishDomainEvents()
    {
        var domainEvents = base.ChangeTracker.Entries<IHasDomainEvents>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                var domainEvents = entity.GetDomainEvents().ToList();   // snapshot of current events
                entity.ClearDomainEvents();                             // then clear them, avoid potential issues with re-entrancy or multiple dispatches
                return domainEvents;
            }).ToList();

        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent);
        }
    }
}
