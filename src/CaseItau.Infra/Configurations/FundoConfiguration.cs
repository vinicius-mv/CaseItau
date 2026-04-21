using CaseItau.Domain.Fundos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CaseItau.Infra.Configurations;

public class FundoConfiguration : IEntityTypeConfiguration<Fundo>
{
    public void Configure(EntityTypeBuilder<Fundo> builder)
    {
        builder.ToTable("FUNDO");

        // FundoCodigo as PK via value converter - EF reads/writes Codigo.Value directly
        builder.Property(f => f.Codigo)
            .HasConversion(
                codigo => codigo.Value,
                str => FundoCodigo.Criar(str).Value!)
            .HasColumnName("CODIGO")
            .HasMaxLength(FundoCodigo.MaxLength)
            .IsRequired();

        builder.HasKey(f => f.Codigo);

        // VO setup - FundoNome mapped to column NOME
        builder.OwnsOne(fundo => fundo.Nome, nomeBuilder =>
        {
            nomeBuilder.Property(n => n.Value)
                .HasColumnName("NOME")
                .HasMaxLength(FundoNome.MaxLength)
                .IsRequired();
        });

        // VO setup - Cnpj mapped to column CNPJ
        builder.OwnsOne(fundo => fundo.Cnpj, cnpjBuilder =>
        {
            cnpjBuilder.Property(c => c.Value)
                .HasColumnName("CNPJ")
                .HasMaxLength(Cnpj.RequiredLength)
                .IsRequired();

            cnpjBuilder.HasIndex(c => c.Value).IsUnique();
        });

        builder.Property(fundo => fundo.Patrimonio)
            .HasColumnName("PATRIMONIO")
            .IsRequired(false);

        builder.Property(fundo => fundo.CodigoTipo)
            .HasColumnName("CODIGO_TIPO")
            .IsRequired();

        // Relation 1:N Fundo - TipoFundo
        builder.HasOne(fundo => fundo.TipoFundo)
            .WithMany()
            .HasForeignKey(fundo => fundo.CodigoTipo)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(fundo => fundo.Id);
    }
}
