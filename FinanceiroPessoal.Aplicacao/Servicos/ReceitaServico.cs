using FinanceiroPessoal.Aplicacao.DTOs;
using FinanceiroPessoal.Aplicacao.Interfaces;
using FinanceiroPessoal.Dominio.Comum;
using FinanceiroPessoal.Dominio.Entidades;
using FinanceiroPessoal.Dominio.Interfaces;
using FluentValidation;
using FluentValidation.Results;

namespace FinanceiroPessoal.Aplicacao.Servicos
{
    public class ReceitaServico : IReceitaServico
    {
        private readonly IRepositorioReceitas _repositorioReceitas;
        private readonly IValidator<CriarReceitaDto> _validadorCriarReceita;
        private readonly IValidator<AtualizarReceitaDto> _validadorAtualizarReceita;

        public ReceitaServico(
            IRepositorioReceitas repositorioReceitas,
            IValidator<CriarReceitaDto> validadorCriacao,
            IValidator<AtualizarReceitaDto> validadorAtualizacao)
        {
            _repositorioReceitas = repositorioReceitas;
            _validadorCriarReceita = validadorCriacao;
            _validadorAtualizarReceita = validadorAtualizacao;
        }

        public async Task<RespostaApi<ReceitaDto>> AdicionarReceita(CriarReceitaDto receitaDto, Guid usuarioId)
        {
            ValidationResult resultado = _validadorCriarReceita.Validate(receitaDto);
            if (!resultado.IsValid)
                return RespostaApi<ReceitaDto>.ErroResposta("Erro de validação", resultado.Errors.Select(e => e.ErrorMessage).ToList());

            var receita = new Receita
            {
                Id = Guid.NewGuid().ToString(),
                Valor = receitaDto.Valor,
                DataRecebimento = receitaDto.Data,
                Descricao = receitaDto.Descricao,
                CategoriaId = receitaDto.CategoriaId,
                UsuarioId = usuarioId,
                DataCriacao = DateTime.UtcNow
            };

            var novaReceita = await _repositorioReceitas.AdicionarAsync(receita);

            return RespostaApi<ReceitaDto>.SucessoResposta(MapearParaDto(novaReceita), "Receita adicionada com sucesso.");
        }

        public async Task<RespostaApi<ReceitaDto>> AtualizarReceita(string id, AtualizarReceitaDto receitaDto, Guid usuarioId)
        {
            ValidationResult resultado = _validadorAtualizarReceita.Validate(receitaDto);
            if (!resultado.IsValid)
                return RespostaApi<ReceitaDto>.ErroResposta("Erro de validação", resultado.Errors.Select(e => e.ErrorMessage).ToList());

            var receitaExistente = await _repositorioReceitas.ObterPorIdAsync(id);
            if (receitaExistente == null || receitaExistente.UsuarioId != usuarioId)
                return RespostaApi<ReceitaDto>.ErroResposta("Receita não encontrada.");

            receitaExistente.Valor = receitaDto.Valor;
            receitaExistente.DataRecebimento = receitaDto.Data;
            receitaExistente.Descricao = receitaDto.Descricao;
            receitaExistente.CategoriaId = receitaDto.CategoriaId;
            receitaExistente.DataAtualizacao = DateTime.UtcNow;

            await _repositorioReceitas.AtualizarAsync(receitaExistente);

            return RespostaApi<ReceitaDto>.SucessoResposta(MapearParaDto(receitaExistente), "Receita atualizada com sucesso.");
        }

        public async Task<RespostaApi<ReceitaDto>> ObterReceitaPorId(string id, Guid usuarioId)
        {
            var receita = await _repositorioReceitas.ObterPorIdAsync(id);
            if (receita == null || receita.UsuarioId != usuarioId)
                return RespostaApi<ReceitaDto>.ErroResposta("Receita não encontrada.");

            return RespostaApi<ReceitaDto>.SucessoResposta(MapearParaDto(receita));
        }

        public async Task<RespostaApi<IEnumerable<ReceitaDto>>> ObterReceitasPorUsuario(Guid usuarioId)
        {
            var receitas = await _repositorioReceitas.ObterPorUsuarioAsync(usuarioId);
            var listaDto = receitas.Select(MapearParaDto);

            return RespostaApi<IEnumerable<ReceitaDto>>.SucessoResposta(listaDto);
        }

        public async Task<RespostaApi<bool>> RemoverReceita(string id, Guid usuarioId)
        {
            var receita = await _repositorioReceitas.ObterPorIdAsync(id);
            if (receita == null || receita.UsuarioId != usuarioId)
                return RespostaApi<bool>.ErroResposta("Receita não encontrada.");

            await _repositorioReceitas.RemoverAsync(id);
            return RespostaApi<bool>.SucessoResposta(true, "Receita removida com sucesso.");
        }

        private static ReceitaDto MapearParaDto(Receita receita)
        {
            return new ReceitaDto
            {
                Id = receita.Id,
                Valor = receita.Valor,
                Data = receita.DataRecebimento,
                Descricao = receita.Descricao,
                CategoriaId = receita.CategoriaId,
                UsuarioId = receita.UsuarioId,
                DataCriacao = receita.DataCriacao,
                DataAtualizacao = receita.DataAtualizacao ?? DateTime.UtcNow
            };
        }
    }
}
