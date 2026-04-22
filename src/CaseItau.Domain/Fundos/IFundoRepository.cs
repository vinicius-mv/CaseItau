namespace CaseItau.Domain.Fundos;

public interface IFundoRepository
{
    Task<Fundo?> ObterAsync(FundoCodigo codigo, CancellationToken cancellationToken = default);
    Task<Fundo?> ObterPorCnpjAsync(Cnpj cnpj, CancellationToken cancellationToken = default);
    Task<TipoFundo?> ObterTipoFundoAsync(int codigoTipo, CancellationToken cancellationToken = default);
    void Adicionar(Fundo fundo);
    void Atualizar(Fundo fundo);
    void Remover(Fundo fundo);
}
