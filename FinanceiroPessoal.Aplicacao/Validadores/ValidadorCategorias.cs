using FluentValidation;
using FinanceiroPessoal.Aplicacao.DTOs;

public class ValidadorCriarCategorias : AbstractValidator<CriarCategoriaDto>
{
    public ValidadorCriarCategorias()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome da categoria é obrigatório.")
            .MaximumLength(50).WithMessage("O nome da categoria pode ter no máximo 50 caracteres.");

        RuleFor(x => x.Tipo)
            .NotEmpty().WithMessage("O tipo da categoria é obrigatório.");

        RuleFor(x => x.Descricao)
            .MaximumLength(200).WithMessage("A descrição pode ter no máximo 200 caracteres.");
    }
}

public class ValidadorAtualizarCategorias : AbstractValidator<AtualizarCategoriaDto>
{
    public ValidadorAtualizarCategorias()
    {
        RuleFor(x => x.Id)
            .GreaterThan("0").WithMessage("O ID da categoria é obrigatório para atualização.");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome da categoria é obrigatório.")
            .MaximumLength(50).WithMessage("O nome da categoria pode ter no máximo 50 caracteres.");

        RuleFor(x => x.Tipo)
            .NotEmpty().WithMessage("O tipo da categoria é obrigatório.");

        RuleFor(x => x.Descricao)
            .MaximumLength(200).WithMessage("A descrição pode ter no máximo 200 caracteres.");
    }
}
