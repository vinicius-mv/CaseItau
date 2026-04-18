using CaseItau.Domain.Abstractions;

namespace CaseItau.Domain.Fundos;

public sealed class Fundo : Entity
{
    private Fundo(string codigo, string nome, Cnpj cnpj, TipoFundo tipoFundo)
    {
        Codigo = codigo;
        Nome = nome;
        Cnpj = cnpj;
        TipoFundo = tipoFundo;
        CodigoTipo = tipoFundo.CodigoTipo;
    }

    // rehydration
    protected Fundo() { }

    public static Result<Fundo> Criar(string codigo, string nome, Cnpj cnpj, TipoFundo tipoFundo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            return Result.Failure<Fundo>(FundoErrors.CodigoObrigatorio);

        if (string.IsNullOrWhiteSpace(nome))
            return Result.Failure<Fundo>(FundoErrors.NomeObrigatorio);

        if (!cnpj.EhValidoCnpj)
            return Result.Failure<Fundo>(FundoErrors.CnpjInvalido(cnpj.Value));

        var fundo = new Fundo(codigo, nome, cnpj, tipoFundo);

        return Result.Success(fundo);
    }

    public string Codigo { get; private set; }
    public string Nome { get; private set; }
    public Cnpj Cnpj { get; private set; }
    public TipoFundo TipoFundo { get; private set; }
    public decimal? Patrimonio { get; private set; }

    // EF Rel
    public int CodigoTipo { get; private set; }

    public void AtualizarPatrimonio(decimal patrimonio)
    {
        Patrimonio = patrimonio;
    }

}
