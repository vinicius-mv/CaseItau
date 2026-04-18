using CaseItau.Domain.Fundos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CaseItau.Infra.Configurations
{
    public class FundoConfiguration : IEntityTypeConfiguration<Fundo>
    {
        public void Configure(EntityTypeBuilder<Fundo> builder)
        {
            builder.ToTable("FUNDO");

            builder.HasKey(fundo => fundo.Codigo);

            builder.Property(fundo => fundo.Codigo).HasColumnName("CODIGO")
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(fundo => fundo.Nome).HasColumnName("NOME")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(fundo => fundo.Patrimonio).HasColumnName("PATRIMONIO")
                .IsRequired(false);

            builder.Property(fundo => fundo.CodigoTipo).HasColumnName("CODIGO_TIPO")
                .IsRequired();

            // VO setup - Cnpj mapped to column CNPJ
            builder.OwnsOne(fundo => fundo.Cnpj, cnpj =>
            {
                cnpj.Property(c => c.Value).HasColumnName("CNPJ")
                    .HasMaxLength(14)
                    .IsRequired();
            });

            // Relation 1:N Fundo - TipoFundo
            builder.HasOne(fundo => fundo.TipoFundo)
                .WithMany()
                .HasForeignKey(fundo => fundo.CodigoTipo)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
