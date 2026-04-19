using CaseItau.Domain.Fundos;
using FluentValidation;

namespace CaseItau.Application.Fundos.RemoverFundo;

public class RemoverFundoCommandValidator : AbstractValidator<RemoverFundoCommand>
{
    public RemoverFundoCommandValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty()
            .WithMessage("Código é obrigatório.")
            .MaximumLength(Fundo.CodigoMaxLength)
            .WithMessage($"Código não pode ter mais de {Fundo.CodigoMaxLength} caracteres.");
    }
}
