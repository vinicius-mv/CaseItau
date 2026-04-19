using CaseItau.Domain.Abstractions;

namespace CaseItau.Domain.Fundos;

public sealed class Fundo : Entity
{
    private Fundo(string codigo, FundoNome nome, Cnpj cnpj, TipoFundo tipoFundo)
    {
        Codigo = codigo;
        Nome = nome;
        Cnpj = cnpj;
        TipoFundo = tipoFundo;
        CodigoTipo = tipoFundo.CodigoTipo;
    }

    // rehydration
    protected Fundo() { }

    public static Result<Fundo> Criar(string codigo, string nome, string cnpj, TipoFundo tipoFundo)
    {
        if (string.IsNullOrEmpty(codigo))
            return Result.Failure<Fundo>(FundoErrors.CodigoObrigatorio);

        if (codigo.Length > CodigoMaxLength)
            return Result.Failure<Fundo>(FundoErrors.CodigoMaiorQuePermitido(codigo));

        var nomeResult = FundoNome.Criar(nome);
        if (nomeResult.IsFailure)
            return Result.Failure<Fundo>(nomeResult.Error);

        var cnpjResult = Cnpj.Criar(cnpj);
        if (cnpjResult.IsFailure)
            return Result.Failure<Fundo>(cnpjResult.Error);

        var fundo = new Fundo(codigo, nomeResult.Value, cnpjResult.Value, tipoFundo);

        return Result.Success(fundo);
    }

    public Result Atualizar(string nome, string cnpj, TipoFundo tipoFundo)
    {
        var nomeResult = FundoNome.Criar(nome);
        if (nomeResult.IsFailure)
            return Result.Failure(nomeResult.Error);

        var cnpjResult = Cnpj.Criar(cnpj);
        if (cnpjResult.IsFailure)
            return Result.Failure(cnpjResult.Error);

        Nome = nomeResult.Value;
        Cnpj = cnpjResult.Value;
        TipoFundo = tipoFundo;
        CodigoTipo = tipoFundo.CodigoTipo;

        return Result.Success();
    }

    public string Codigo { get; private set; }
    public const int CodigoMaxLength = 20;

    public FundoNome Nome { get; private set; }
    public Cnpj Cnpj { get; private set; }
    public TipoFundo TipoFundo { get; private set; }
    public decimal? Patrimonio { get; private set; }

    // EF Rel
    public int CodigoTipo { get; private set; }

    public void MovimentarPatrimonio(decimal patrimonio)
    {
        if (Patrimonio is null) 
            Patrimonio = 0;
        
        Patrimonio += patrimonio;
    }
}
