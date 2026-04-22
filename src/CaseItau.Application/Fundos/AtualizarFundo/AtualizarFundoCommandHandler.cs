using CaseItau.Application.Abstractions.Messaging;
using CaseItau.Domain.Abstractions;
using CaseItau.Domain.Fundos;

namespace CaseItau.Application.Fundos.AtualizarFundo;

public sealed class AtualizarFundoCommandHandler : ICommandHandler<AtualizarFundoCommand>
{
    private readonly IFundoRepository _fundoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AtualizarFundoCommandHandler(IFundoRepository fundoRepository, IUnitOfWork unitOfWork)
    {
        _fundoRepository = fundoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AtualizarFundoCommand request, CancellationToken cancellationToken)
    {
        var fundoCodigoResult = FundoCodigo.Criar(request.Codigo);
        if (fundoCodigoResult.IsFailure)
            return Result.Failure(fundoCodigoResult.Error);

        var fundo = await _fundoRepository.ObterAsync(fundoCodigoResult.Value, cancellationToken);
        if (fundo is null)
            return Result.Failure(FundoErrors.NaoEncontrado(request.Codigo));

        var cnpj = Cnpj.Criar(request.Cnpj);
        if (cnpj.IsFailure)
            return Result.Failure(cnpj.Error);

        var fundoPorCnpj = await _fundoRepository.ObterPorCnpjAsync(cnpj.Value, cancellationToken);
        if (fundoPorCnpj is not null && 
            fundoPorCnpj.Codigo != fundo.Codigo)
            return Result.Failure(FundoErrors.CnpjIndisponivel(request.Cnpj));

        var tipoFundo = await _fundoRepository.ObterTipoFundoAsync(request.CodigoTipo, cancellationToken);
        if (tipoFundo is null)
            return Result.Failure(FundoErrors.TipoFundoInvalido(request.CodigoTipo));

        var result = fundo.Atualizar(request.Nome, request.Cnpj, tipoFundo);
        if (result.IsFailure)
            return Result.Failure(result.Error);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
