using CaseItau.Application.Abstractions.Messaging;

namespace CaseItau.Application.Fundos.ObterFundo;

public sealed record ObterFundoQuery(string Codigo)
    : IQuery<FundoResponse>
{
}
