namespace CaseItau.API.Controllers.Fundos;

public record AdicionarFundoRequest(
    string Codigo,
    string Nome,
    string Cnpj,
    int CodigoTipo
);
