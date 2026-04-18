using CaseItau.Domain.Fundos;
using Microsoft.EntityFrameworkCore;

namespace CaseItau.Infra.Repositories;

internal sealed class FundoRepository : IFundoRepository
{
    private readonly ApplicationDbContext _dbContext;

    public FundoRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Fundo?> ObterAsync(string codigo, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Fundos.FirstOrDefaultAsync(f => f.Codigo == codigo, cancellationToken);
    }

    public async Task<TipoFundo?> ObterTipoFundoAsync(int codigoTipo, CancellationToken cancellationToken)
    {
        return await _dbContext.TipoFundos.FirstOrDefaultAsync(f => f.CodigoTipo == codigoTipo, cancellationToken);
    }

    public void Adicionar(Fundo fundo)
    {
        _dbContext.Add(fundo);
    }
}
