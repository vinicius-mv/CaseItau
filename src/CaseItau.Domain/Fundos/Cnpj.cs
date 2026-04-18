using CaseItau.Domain.Abstractions;

namespace CaseItau.Domain.Fundos;

public record Cnpj
{
    public const int RequiredLength = 14;

    public string Value { get; }

    private Cnpj(string value)
    {
        Value = value;
    }

    // rehydration
    protected Cnpj() { }

    public static Result<Cnpj> Criar(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<Cnpj>(FundoErrors.CnpjInvalido(value ?? string.Empty));

        var digitos = ObterDigitos(value);

        if (!EhValido(digitos))
            return Result.Failure<Cnpj>(FundoErrors.CnpjInvalido(value));

        return Result.Success(new Cnpj(digitos));
    }

    /// <summary>
    /// Remove any non-digit characters (dots, slashes, dashes, spaces).
    /// </summary>
    private static string ObterDigitos(string value) =>
        new string(value.Where(char.IsDigit).ToArray());

    /// <summary>
    /// Validates CNPJ using the standard two-digit verification algorithm.
    /// </summary>
    private static bool EhValido(string digitos)
    {
        if (digitos.Length != RequiredLength)
            return false;

        // Reject sequences of identical digits (e.g. 00000000000000)
        if (digitos.Distinct().Count() == 1)
            return false;

        return ChecarDigito(digitos, 12) && ChecarDigito(digitos, 13);
    }

    private static bool ChecarDigito(string digitos, int posicao)
    {
        int[] pesos = posicao == 12
            ? new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 }
            : new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        int soma = digitos
            .Take(posicao)
            .Select((c, i) => (c - '0') * pesos[i])
            .Sum();

        int resto = soma % 11;
        int esperado = resto < 2 ? 0 : 11 - resto;

        return (digitos[posicao] - '0') == esperado;
    }

    public override string ToString() => Value;
}
