using CaseItau.Application.Abstractions.Messaging;
using CaseItau.Domain.Abstractions;

namespace CaseItau.Application.Fundos.ListarFundos;

internal sealed class ListarFundosQueryHandler : IQueryHandler<ListarFundosQuery, IReadOnlyList<FundoResponse>>
{
    private readonly IFundoReadRepository _fundosReadRepository;

    public ListarFundosQueryHandler(IFundoReadRepository fundosReadRepository)
    {
        _fundosReadRepository = fundosReadRepository;
    }

    public async Task<Result<IReadOnlyList<FundoResponse>>> Handle(ListarFundosQuery request, CancellationToken cancellationToken)
    {
        var fundos = await _fundosReadRepository.ListarAsync(cancellationToken);

        return Result.Success<IReadOnlyList<FundoResponse>>(fundos);
    }
}
