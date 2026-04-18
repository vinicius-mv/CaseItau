using CaseItau.Application.Abstractions.Messaging;

namespace CaseItau.Application.Fundos.MovimentarPatrimonioFundo;

public record MovimentarPatrimonioFundoCommand(
    string Codigo,
    decimal ValorMovimentacao
    )
    : ICommand;
