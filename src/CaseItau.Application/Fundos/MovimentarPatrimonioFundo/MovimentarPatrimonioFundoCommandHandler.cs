using CaseItau.Application.Abstractions.Messaging;
using CaseItau.Domain.Abstractions;
using CaseItau.Domain.Fundos;

namespace CaseItau.Application.Fundos.MovimentarPatrimonioFundo;

public sealed class MovimentarPatrimonioFundoCommandHandler : ICommandHandler<MovimentarPatrimonioFundoCommand>
{
    private readonly IFundoRepository _fundoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MovimentarPatrimonioFundoCommandHandler(IFundoRepository fundoRepository, IUnitOfWork unitOfWork)
    {
        _fundoRepository = fundoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(MovimentarPatrimonioFundoCommand request, CancellationToken cancellationToken)
    {
        var fundo = await _fundoRepository.ObterAsync(request.Codigo, cancellationToken);
        if (fundo is null)
            return Result.Failure<string>(FundoErrors.NaoEncontrado(request.Codigo));

        fundo.MovimentarPatrimonio(request.ValorMovimentacao);

        _fundoRepository.Atualizar(fundo);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}