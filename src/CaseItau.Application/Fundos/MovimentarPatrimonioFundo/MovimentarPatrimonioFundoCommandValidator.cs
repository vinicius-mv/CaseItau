using CaseItau.Domain.Fundos;
using FluentValidation;

namespace CaseItau.Application.Fundos.MovimentarPatrimonioFundo;

public class MovimentarPatrimonioFundoCommandValidator : AbstractValidator<MovimentarPatrimonioFundoCommand>
{
    public MovimentarPatrimonioFundoCommandValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty()
            .WithMessage("Código é obrigatório.")
            .MaximumLength(FundoCodigo.MaxLength)
            .WithMessage($"Código não pode ter mais de {FundoCodigo.MaxLength} caracteres.");

        RuleFor(x => x.ValorMovimentacao)
            .NotEqual(0)
            .WithMessage("Valor de movimentação não pode ser zero.");
    }
}
