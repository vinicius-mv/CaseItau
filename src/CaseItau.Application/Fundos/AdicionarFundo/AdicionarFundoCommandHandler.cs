using CaseItau.Application.Abstractions.Messaging;
using CaseItau.Domain.Abstractions;
using CaseItau.Domain.Fundos;
using MediatR;

namespace CaseItau.Application.Fundos.AdicionarFundo;

public class AdicionarFundoCommandHandler : ICommandHandler<AdicionarFundoCommand, string>
{
    private readonly IFundoRepository _fundoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AdicionarFundoCommandHandler(IFundoRepository fundoRepository, IUnitOfWork unitOfWork)
    {
        _fundoRepository = fundoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string>> Handle(AdicionarFundoCommand request, CancellationToken cancellationToken)
    {
        var fundo = await _fundoRepository.ObterAsync(request.Codigo, cancellationToken);
        if (fundo is not null)
            return Result.Failure<string>(FundoErrors.JaExiste(request.Codigo));

        var tipoFundo = await _fundoRepository.ObterTipoFundoAsync(request.CodigoTipo, cancellationToken);
        if (tipoFundo is null)
            return Result.Failure<string>(FundoErrors.TipoFundoInvalido(request.CodigoTipo));

        var fundoResult = Fundo.Criar(request.Codigo, request.Nome, new Cnpj(request.Cnpj), tipoFundo);

        if (fundoResult.IsFailure)
            return Result.Failure<string>(fundoResult.Error);

        _fundoRepository.Adicionar(fundoResult.Value);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success<string>(fundoResult.Value.Codigo);
    }
}
