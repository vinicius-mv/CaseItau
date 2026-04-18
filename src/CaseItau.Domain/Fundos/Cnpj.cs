namespace CaseItau.Domain.Fundos;

public record Cnpj
{
    public string Value { get; }

    public Cnpj(string value)
    {
        var digits = Strip(value);

        if (!IsValid(digits))
            throw new ApplicationException("CNPJ inválido.");

        Value = digits;
    }

    // rehydration
    protected Cnpj(string value, bool skipValidation)
    {
        Value = Strip(value);
    }

    /// <summary>
    /// Remove any non-digit characters (dots, slashes, dashes, spaces).
    /// </summary>
    private static string Strip(string value) =>
        new string(value.Where(char.IsDigit).ToArray());

    /// <summary>
    /// Validates CNPJ using the standard two-digit verification algorithm.
    /// </summary>
    private static bool IsValid(string digits)
    {
        if (digits.Length != 14)
            return false;

        // Reject sequences of identical digits (e.g. 00000000000000)
        if (digits.Distinct().Count() == 1)
            return false;

        return CheckDigit(digits, 12) && CheckDigit(digits, 13);
    }

    private static bool CheckDigit(string digits, int position)
    {
        int[] weights = position == 12
            ? new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 }
            : new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        int sum = digits
            .Take(position)
            .Select((c, i) => (c - '0') * weights[i])
            .Sum();

        int remainder = sum % 11;
        int expected = remainder < 2 ? 0 : 11 - remainder;

        return (digits[position] - '0') == expected;
    }

    public override string ToString() => Value;
}
