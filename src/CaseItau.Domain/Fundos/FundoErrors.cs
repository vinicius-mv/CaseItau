using CaseItau.Domain.Abstractions;

namespace CaseItau.Domain.Fundos;

public static class FundoErrors
{
    public static Error NaoEncontrado(string fundoCodigo) =>
        Error.NotFound(
            code: "Fundo.NaoEncontrado",
            description: $"O fundo com código {fundoCodigo} não foi encontrado");

    public static Error JaExiste(string fundoCodigo) =>
        Error.Conflict(
            code: "Fundo.JaExiste",
            description: $"O fundo com código {fundoCodigo} já foi registrado");

    public static Error TipoFundoInvalido(int tipoFundo) =>
        Error.Validation(
            code: "Fundo.TipoFundoInvalido",
            description: $"O tipo de fundo {tipoFundo} é inválido");

    public static readonly Error CodigoObrigatorio =
        Error.Validation(
            code: "Fundo.CodigoObrigatorio",
            description: "O código do fundo é obrigatório.");

    public static Error CodigoMaiorQuePermitido(string codigo) =>
        Error.Validation(
            code: "Fundo.CodigoMaiorQuePermitido",
            description: $"O código {codigo} é maior que o permitido");

    public static Error NomeMaiorQuePermitido(string nome) =>
        Error.Validation(
            code: "Fundo.NomeMaiorQuePermitido",
            description: $"O nome {nome} é maior que o permitido");

    public static Error CnpjInvalido(string cnpj) =>
        Error.Validation(
            code: "Fundo.CnpjInvalido",
            description: $"O CNPJ {cnpj} é inválido");

    public static readonly Error NomeObrigatorio =
        Error.Validation(
            code: "Fundo.NomeObrigatorio",
            description: "O nome do fundo é obrigatório.");
}
