using CaseItau.Domain.Fundos;

namespace CaseItau.Domain.UnitTests.Fundos;

internal static class FundoData
{
    public static FundoCodigo FundoCodigo = FundoCodigo.Criar("FUNDORF01").Value;
    public static FundoNome FundoNome = FundoNome.Criar("FUNDO DE INVESTIMENTO XYZ").Value;
    public static Cnpj Cnpj = Cnpj.Criar("74.353.513/0001-12").Value;
    public static TipoFundo TipoFundo = new TipoFundo(1, "ACOES");

    public static Fundo CriarFundoValido()
    {
        return Fundo.Criar(FundoCodigo.Value, FundoNome.Value, Cnpj.Value, TipoFundo).Value;
    }
}
