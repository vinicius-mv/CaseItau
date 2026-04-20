namespace CaseItau.Domain.Fundos;

public interface IFundoRepository
{
    Task<Fundo?> ObterAsync(string codigo, CancellationToken cancellationToken = default);
    Task<Fundo?> ObterPorCnpjAsync(string cnpj, CancellationToken cancellationToken = default);
    Task<TipoFundo?> ObterTipoFundoAsync(int codigoTipo, CancellationToken cancellationToken);
    void Adicionar(Fundo fundo);
    void Atualizar(Fundo fundo);
    void Remover(Fundo fundo);
}
