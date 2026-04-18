namespace CaseItau.API.Controllers.Fundos;

public record AtualizarFundoRequest(
    string Nome,
    string Cnpj,
    int CodigoTipo
);
