using CaseItau.Application.Abstractions.Messaging;

namespace CaseItau.Application.Fundos.AdicionarFundo;

public sealed record AdicionarFundoCommand(
    string Codigo,
    string Nome,
    string Cnpj,
    int CodigoTipo) 
    : ICommand<string>;
