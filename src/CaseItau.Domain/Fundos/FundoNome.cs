using CaseItau.Domain.Abstractions;

namespace CaseItau.Domain.Fundos;

public class FundoNome
{
    public const int MaxLength = 100;

    public string Value { get; }

    private FundoNome(string value)
    {
        Value = value;
    }

    // rehydration
    protected FundoNome() { }

    public static Result<FundoNome> Criar(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<FundoNome>(FundoErrors.NomeObrigatorio);

        value = value.Trim();

        if (value.Length > MaxLength)
            return Result.Failure<FundoNome>(FundoErrors.NomeMaiorQuePermitido(value));

        var fundoNome = new FundoNome(value);

        return Result.Success(fundoNome);
    }

    public override string ToString() => Value;
}
