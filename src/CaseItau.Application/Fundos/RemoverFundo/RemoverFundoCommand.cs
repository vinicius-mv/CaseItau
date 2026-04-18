using CaseItau.Application.Abstractions.Messaging;

namespace CaseItau.Application.Fundos.RemoverFundo;

public record RemoverFundoCommand(string Codigo) : ICommand
{
}
