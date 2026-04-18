using CaseItau.Domain.Abstractions;
using CaseItau.Domain.Fundos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CaseItau.Infra;

public class ApplicationDbContext : DbContext, IUnitOfWork
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {
    }

    public DbSet<Fundo> Fundos { get; private set; }

    public DbSet<TipoFundo> TipoFundos { get; private set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
