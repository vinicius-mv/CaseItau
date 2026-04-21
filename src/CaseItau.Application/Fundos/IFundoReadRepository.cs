namespace CaseItau.Application.Fundos;

public interface IFundoReadRepository
{
    Task<IReadOnlyList<FundoResponse>> ListarAsync(CancellationToken cancellationToken = default);
    Task<FundoResponse?> ObterAsync(string codigo, CancellationToken cancellationToken = default);
}
