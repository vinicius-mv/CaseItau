using CaseItau.Application.Abstractions.Messaging;
using CaseItau.Domain.Abstractions;
using CaseItau.Domain.Fundos;

namespace CaseItau.Application.Fundos.RemoverFundo;

public sealed class RemoverFundoCommandHandler : ICommandHandler<RemoverFundoCommand>
{
    private readonly IFundoRepository _fundoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoverFundoCommandHandler(IFundoRepository fundoRepository, IUnitOfWork unitOfWork)
    {
        _fundoRepository = fundoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RemoverFundoCommand request, CancellationToken cancellationToken)
    {
        var fundo = await _fundoRepository.ObterAsync(request.Codigo, cancellationToken);
        if (fundo is null)
            return Result.Failure(FundoErrors.NaoEncontrado(request.Codigo));

        _fundoRepository.Remover(fundo);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
