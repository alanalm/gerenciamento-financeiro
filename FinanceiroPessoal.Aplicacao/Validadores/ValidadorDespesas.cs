using FinanceiroPessoal.Aplicacao.DTOs;
using FluentValidation;
using System;

namespace FinanceiroPessoal.Aplicacao.Validadores
{
    public class ValidadorCriarDespesas : AbstractValidator<CriarDespesaDto>
    {
        public ValidadorCriarDespesas()
        {
            RuleFor(x => x.Valor)
                .GreaterThan(0).WithMessage("O valor da despesa deve ser maior que zero.");

            RuleFor(x => x.Data)
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("A data não pode ser no futuro.");

            RuleFor(x => x.CategoriaId)
                .NotEmpty().WithMessage("A categoria é obrigatória.");

            RuleFor(x => x.Descricao)
                .MaximumLength(200).WithMessage("A descrição pode ter no máximo 200 caracteres.");
        }
    }

    public class ValidadorAtualizarDespesas : AbstractValidator<AtualizarDespesaDto>
    {
        public ValidadorAtualizarDespesas()
        {
            RuleFor(x => x.Valor)
                .GreaterThan(0).WithMessage("O valor da despesa deve ser maior que zero.");

            RuleFor(x => x.Data)
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("A data não pode ser no futuro.");

            RuleFor(x => x.CategoriaId)
                .NotEmpty().WithMessage("A categoria é obrigatória.");

            RuleFor(x => x.Descricao)
                .MaximumLength(200).WithMessage("A descrição pode ter no máximo 200 caracteres.");
        }
    }
}
