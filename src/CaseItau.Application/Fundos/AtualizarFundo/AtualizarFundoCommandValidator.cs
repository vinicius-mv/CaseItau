using CaseItau.Domain.Fundos;
using FluentValidation;

namespace CaseItau.Application.Fundos.AtualizarFundo;

public class AtualizarFundoCommandValidator : AbstractValidator<AtualizarFundoCommand>
{
    public AtualizarFundoCommandValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty()
            .WithMessage("Código é obrigatório.")
            .MaximumLength(Fundo.CodigoMaxLength)
            .WithMessage($"Código não pode ter mais de {Fundo.CodigoMaxLength} caracteres.");

        RuleFor(x => x.Nome)
            .NotEmpty()
            .WithMessage("Nome é obrigatório.")
            .MaximumLength(FundoNome.MaxLength)
            .WithMessage($"Nome não pode ter mais de {FundoNome.MaxLength} caracteres.");

        RuleFor(x => x.Cnpj)
            .NotEmpty()
            .WithMessage("CNPJ é obrigatório.")
            .Must(cnpj => new string(cnpj.Where(char.IsDigit).ToArray()).Length == Cnpj.RequiredLength)
            .WithMessage($"CNPJ deve conter {Cnpj.RequiredLength} dígitos.");

        RuleFor(x => x.CodigoTipo)
            .GreaterThan(0)
            .WithMessage("Código do tipo de fundo é obrigatório.");
    }
}
