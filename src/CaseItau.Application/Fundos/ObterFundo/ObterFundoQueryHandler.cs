using CaseItau.Application.Abstractions.Messaging;
using CaseItau.Domain.Abstractions;
using CaseItau.Domain.Fundos;

namespace CaseItau.Application.Fundos.ObterFundo;

internal sealed class ObterFundoQueryHandler : IQueryHandler<ObterFundoQuery, FundoResponse>
{
    private readonly IFundoReadRepository _fundosReadRepository;

    public ObterFundoQueryHandler(IFundoReadRepository fundosReadRepository)
    {
        _fundosReadRepository = fundosReadRepository;
    }

    public async Task<Result<FundoResponse>> Handle(ObterFundoQuery request, CancellationToken cancellationToken)
    {
        var fundo = await _fundosReadRepository.ObterAsync(request.Codigo, cancellationToken);

        if (fundo is null)
            return Result.Failure<FundoResponse>(FundoErrors.NaoEncontrado(request.Codigo));

        return fundo;
    }
}
