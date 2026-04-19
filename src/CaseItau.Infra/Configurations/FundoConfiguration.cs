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
            builder.Ignore(fundo => fundo.Id); // Ignore the property Id, since CodigoTipo is the primary key

            builder.Property(fundo => fundo.Codigo)
                .HasColumnName("CODIGO")
                .HasMaxLength(Fundo.CodigoMaxLength)
                .IsRequired();

            // VO setup - FundoNome mapped to column NOME
            builder.OwnsOne(fundo => fundo.Nome, nome =>
            {
                nome.Property(n => n.Value).HasColumnName("NOME")
                    .HasMaxLength(FundoNome.MaxLength)
                    .IsRequired();
            });

            // VO setup - Cnpj mapped to column CNPJ
            builder.OwnsOne(fundo => fundo.Cnpj, cnpj =>
            {
                cnpj.Property(c => c.Value).HasColumnName("CNPJ")
                    .HasMaxLength(Cnpj.RequiredLength)
                    .IsRequired();

                cnpj.HasIndex(c => c.Value).IsUnique();
            });

            builder.Property(fundo => fundo.Patrimonio).HasColumnName("PATRIMONIO")
                .IsRequired(false);

            builder.Property(fundo => fundo.CodigoTipo).HasColumnName("CODIGO_TIPO")
                .IsRequired();

            // Relation 1:N Fundo - TipoFundo
            builder.HasOne(fundo => fundo.TipoFundo)
                .WithMany()
                .HasForeignKey(fundo => fundo.CodigoTipo)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
