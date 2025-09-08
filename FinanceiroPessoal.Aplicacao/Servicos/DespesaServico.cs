using FinanceiroPessoal.Aplicacao.DTOs;
using FinanceiroPessoal.Aplicacao.Interfaces;
using FinanceiroPessoal.Dominio.Comum;
using FinanceiroPessoal.Dominio.Entidades;
using FinanceiroPessoal.Dominio.Interfaces;
using FluentValidation;
using FluentValidation.Results;

namespace FinanceiroPessoal.Aplicacao.Servicos
{
    public class DespesaServico : IDespesaServico
    {
        private readonly IRepositorioDespesas _repositorioDespesas;
        private readonly IValidator<CriarDespesaDto> _validadorCriarDespesa;
        private readonly IValidator<AtualizarDespesaDto> _validadorAtualizarDespesa;

        public DespesaServico(
            IRepositorioDespesas repositorioDespesas,
            IValidator<CriarDespesaDto> validadorCriacao,
            IValidator<AtualizarDespesaDto> validadorAtualizacao)
        {
            _repositorioDespesas = repositorioDespesas;
            _validadorCriarDespesa = validadorCriacao;
            _validadorAtualizarDespesa = validadorAtualizacao;
        }

        public async Task<RespostaApi<DespesaDto>> AdicionarDespesa(CriarDespesaDto despesaDto, Guid usuarioId)
        {
            ValidationResult resultado = _validadorCriarDespesa.Validate(despesaDto);
            if (!resultado.IsValid)
                return RespostaApi<DespesaDto>.ErroResposta("Erro de validação", resultado.Errors.Select(e => e.ErrorMessage).ToList());

            var despesa = new Despesa
            {
                Id = Guid.NewGuid().ToString(),
                Valor = despesaDto.Valor,
                DataPagamento = despesaDto.Data,
                Descricao = despesaDto.Descricao,
                CategoriaId = despesaDto.CategoriaId,
                UsuarioId = usuarioId,
                DataCriacao = DateTime.UtcNow
            };

            var novaDespesa = await _repositorioDespesas.AdicionarAsync(despesa);

            return RespostaApi<DespesaDto>.SucessoResposta(MapearParaDto(novaDespesa), "Despesa adicionada com sucesso.");
        }

        public async Task<RespostaApi<DespesaDto>> AtualizarDespesa(string id, AtualizarDespesaDto despesaDto, Guid usuarioId)
        {
            ValidationResult resultado = _validadorAtualizarDespesa.Validate(despesaDto);
            if (!resultado.IsValid)
                return RespostaApi<DespesaDto>.ErroResposta("Erro de validação", resultado.Errors.Select(e => e.ErrorMessage).ToList());

            var despesaExistente = await _repositorioDespesas.ObterPorIdAsync(id);
            if (despesaExistente == null || despesaExistente.UsuarioId != usuarioId)
                return RespostaApi<DespesaDto>.ErroResposta("Despesa não encontrada.");

            despesaExistente.Valor = despesaDto.Valor;
            despesaExistente.DataPagamento = despesaDto.Data;
            despesaExistente.Descricao = despesaDto.Descricao;
            despesaExistente.CategoriaId = despesaDto.CategoriaId;
            despesaExistente.DataAtualizacao = DateTime.UtcNow;

            await _repositorioDespesas.AtualizarAsync(despesaExistente);

            return RespostaApi<DespesaDto>.SucessoResposta(MapearParaDto(despesaExistente), "Despesa atualizada com sucesso.");
        }

        public async Task<RespostaApi<DespesaDto>> ObterDespesaPorId(string id, Guid usuarioId)
        {
            var despesa = await _repositorioDespesas.ObterPorIdAsync(id);
            if (despesa == null || despesa.UsuarioId != usuarioId)
                return RespostaApi<DespesaDto>.ErroResposta("Despesa não encontrada.");

            return RespostaApi<DespesaDto>.SucessoResposta(MapearParaDto(despesa));
        }

        public async Task<RespostaApi<IEnumerable<DespesaDto>>> ObterDespesasPorUsuario(Guid usuarioId)
        {
            var despesas = await _repositorioDespesas.ObterPorUsuarioAsync(usuarioId);
            var listaDto = despesas.Select(MapearParaDto);

            return RespostaApi<IEnumerable<DespesaDto>>.SucessoResposta(listaDto);
        }

        public async Task<RespostaApi<bool>> RemoverDespesa(string id, Guid usuarioId)
        {
            var despesa = await _repositorioDespesas.ObterPorIdAsync(id);
            if (despesa == null || despesa.UsuarioId != usuarioId)
                return RespostaApi<bool>.ErroResposta("Despesa não encontrada.");

            await _repositorioDespesas.RemoverAsync(id);
            return RespostaApi<bool>.SucessoResposta(true, "Despesa removida com sucesso.");
        }

        private static DespesaDto MapearParaDto(Despesa despesa)
        {
            return new DespesaDto
            {
                Id = despesa.Id,
                Valor = despesa.Valor,
                Data = despesa.DataPagamento,
                Descricao = despesa.Descricao,
                CategoriaId = despesa.CategoriaId,
                UsuarioId = despesa.UsuarioId,
                DataCriacao = despesa.DataCriacao,
                DataAtualizacao = despesa.DataAtualizacao ?? DateTime.UtcNow
            };
        }
    }
}
