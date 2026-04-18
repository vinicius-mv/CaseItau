namespace CaseItau.Domain.Fundos;

public record Cnpj
{
    public string Value { get; }

    public Cnpj(string value)
    {
        var digitos = ObterDigitos(value);

        if (!EhValido(digitos))
            throw new ApplicationException("CNPJ inválido.");

        Value = digitos;
    }

    // rehydration
    protected Cnpj()
    {
    }

    public bool EhValidoCnpj => EhValido(Value);

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
        if (digitos.Length != 14)
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
