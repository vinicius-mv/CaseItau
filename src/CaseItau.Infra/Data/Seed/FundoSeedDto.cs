namespace CaseItau.Infra.Data.Seed;

public partial class DatabaseSeeder
{
    private sealed record FundoSeedDto(
        string Codigo,
        string Nome,
        string Cnpj,
        int CodigoTipo,
        decimal? Patrimonio);
}
