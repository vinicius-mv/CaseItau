using CaseItau.Domain.Fundos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CaseItau.Infra.Configurations;

public class TipoFundoConfiguration : IEntityTypeConfiguration<TipoFundo>
{
    public void Configure(EntityTypeBuilder<TipoFundo> builder)
    {
        builder.ToTable("TIPO_FUNDO");

        builder.HasKey(tipoFundo => tipoFundo.CodigoTipo);
        builder.Ignore(tipoFundo => tipoFundo.Id); // Ignore the property Id, since CodigoTipo is the primary key

        builder.Property(tipoFundo => tipoFundo.CodigoTipo).HasColumnName("CODIGO")
            .IsRequired();

        builder.Property(tipoFundo => tipoFundo.NomeTipo).HasColumnName("NOME")
            .HasMaxLength(20)
            .IsRequired();
    }
}
