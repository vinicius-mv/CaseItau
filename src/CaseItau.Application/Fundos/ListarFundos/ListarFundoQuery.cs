using CaseItau.Application.Abstractions.Messaging;

namespace CaseItau.Application.Fundos.ListarFundos;

public sealed record ListarFundosQuery()
    : IQuery<IReadOnlyList<FundoResponse>>
{
}
