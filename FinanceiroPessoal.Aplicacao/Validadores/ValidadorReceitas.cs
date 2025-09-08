using FinanceiroPessoal.Aplicacao.DTOs;
using FluentValidation;
using System;

namespace FinanceiroPessoal.Aplicacao.Validadores
{
    // Validador para criar receita
    public class ValidadorCriarReceitas : AbstractValidator<CriarReceitaDto>
    {
        public ValidadorCriarReceitas()
        {
            RuleFor(x => x.Valor)
                .GreaterThan(0).WithMessage("O valor da receita deve ser maior que zero.");

            RuleFor(x => x.Data)
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("A data não pode ser no futuro.");

            RuleFor(x => x.CategoriaId)
                .NotEmpty().WithMessage("A categoria é obrigatória.");

            RuleFor(x => x.Descricao)
                .MaximumLength(200).WithMessage("A descrição pode ter no máximo 200 caracteres.");
        }
    }

    // Validador para atualizar receita
    public class ValidadorAtualizarReceitas : AbstractValidator<AtualizarReceitaDto>
    {
        public ValidadorAtualizarReceitas()
        {
            RuleFor(x => x.Valor)
                .GreaterThan(0).WithMessage("O valor da receita deve ser maior que zero.");

            RuleFor(x => x.Data)
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("A data não pode ser no futuro.");

            RuleFor(x => x.CategoriaId)
                .NotEmpty().WithMessage("A categoria é obrigatória.");

            RuleFor(x => x.Descricao)
                .MaximumLength(200).WithMessage("A descrição pode ter no máximo 200 caracteres.");
        }
    }
}
