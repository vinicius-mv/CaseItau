using CaseItau.Domain.Abstractions;

namespace CaseItau.Domain.Fundos;

public class FundoCodigo
{
    public const int MaxLength = 20;

    public string Value { get; }

    private FundoCodigo(string value)
    {
        Value = value;
    }

    // rehydration
    protected FundoCodigo() { }

    public static Result<FundoCodigo> Criar(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<FundoCodigo>(FundoErrors.CodigoObrigatorio);

        value = value.Trim().ToUpper();

        if (value.Length > MaxLength)
            return Result.Failure<FundoCodigo>(FundoErrors.CodigoMaiorQuePermitido(value));

        return Result.Success(new FundoCodigo(value));
    }

    public override string ToString() => Value;
}
