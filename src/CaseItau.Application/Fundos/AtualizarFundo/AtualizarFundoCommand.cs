using CaseItau.Application.Abstractions.Messaging;

namespace CaseItau.Application.Fundos.AtualizarFundo;

public sealed record AtualizarFundoCommand(
    string Codigo,
    string Nome,
    string Cnpj,
    int CodigoTipo)
    : ICommand;
