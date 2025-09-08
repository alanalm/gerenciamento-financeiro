using FinanceiroPessoal.Aplicacao.DTOs;
using FinanceiroPessoal.Aplicacao.Interfaces;
using FinanceiroPessoal.Dominio.Comum;
using FinanceiroPessoal.Dominio.Entidades;
using FinanceiroPessoal.Dominio.Interfaces;
using FluentValidation;
using FluentValidation.Results;

namespace FinanceiroPessoal.Aplicacao.Servicos
{
    public class CategoriaServico : ICategoriaServico
    {
        private readonly IRepositorioCategorias _repositorioCategorias;
        private readonly IValidator<CriarCategoriaDto> _validadorCriarCategoria;
        private readonly IValidator<AtualizarCategoriaDto> _validadorAtualizarCategoria;

        public CategoriaServico(
            IRepositorioCategorias repositorioCategorias,
            IValidator<CriarCategoriaDto> validadorCriacao,
            IValidator<AtualizarCategoriaDto> validadorAtualizacao)
        {
            _repositorioCategorias = repositorioCategorias;
            _validadorCriarCategoria = validadorCriacao;
            _validadorAtualizarCategoria = validadorAtualizacao;
        }

        public async Task<RespostaApi<CategoriaDto>> AdicionarCategoria(CriarCategoriaDto categoriaDto, Guid usuarioId)
        {
            ValidationResult resultado = _validadorCriarCategoria.Validate(categoriaDto);
            if (!resultado.IsValid)
                return RespostaApi<CategoriaDto>.ErroResposta("Erro de validação", resultado.Errors.Select(e => e.ErrorMessage).ToList());

            var categoria = new Categoria
            {
                Id = Guid.NewGuid().ToString(),
                Nome = categoriaDto.Nome,
                Descricao = categoriaDto.Descricao,
                Tipo = categoriaDto.Tipo,
                UsuarioId = usuarioId,
                DataCriacao = DateTime.UtcNow
            };

            var novaCategoria = await _repositorioCategorias.AdicionarAsync(categoria);

            return RespostaApi<CategoriaDto>.SucessoResposta(MapearParaDto(novaCategoria), "Categoria adicionada com sucesso.");
        }

        public async Task<RespostaApi<CategoriaDto>> AtualizarCategoria(string id, AtualizarCategoriaDto categoriaDto, Guid usuarioId)
        {
            ValidationResult resultado = _validadorAtualizarCategoria.Validate(categoriaDto);
            if (!resultado.IsValid)
                return RespostaApi<CategoriaDto>.ErroResposta("Erro de validação", resultado.Errors.Select(e => e.ErrorMessage).ToList());

            var categoriaExistente = await _repositorioCategorias.ObterPorIdAsync(id);
            if (categoriaExistente == null || categoriaExistente.UsuarioId != usuarioId)
                return RespostaApi<CategoriaDto>.ErroResposta("Categoria não encontrada.");

            categoriaExistente.Nome = categoriaDto.Nome;
            categoriaExistente.Tipo = categoriaDto.Tipo;
            categoriaExistente.Descricao = categoriaDto.Descricao;
            categoriaExistente.DataAtualizacao = DateTime.UtcNow;

            await _repositorioCategorias.AtualizarAsync(categoriaExistente);

            return RespostaApi<CategoriaDto>.SucessoResposta(MapearParaDto(categoriaExistente), "Categoria atualizada com sucesso.");
        }

        public async Task<RespostaApi<CategoriaDto>> ObterCategoriaPorId(string id, Guid usuarioId)
        {
            var categoria = await _repositorioCategorias.ObterPorIdAsync(id);
            if (categoria == null || categoria.UsuarioId != usuarioId)
                return RespostaApi<CategoriaDto>.ErroResposta("Categoria não encontrada.");

            return RespostaApi<CategoriaDto>.SucessoResposta(MapearParaDto(categoria));
        }

        public async Task<RespostaApi<IEnumerable<CategoriaDto>>> ObterTodasCategorias(Guid usuarioId)
        {
            var categorias = await _repositorioCategorias.ObterPorUsuarioAsync(usuarioId);
            var listaDto = categorias.Select(MapearParaDto);

            return RespostaApi<IEnumerable<CategoriaDto>>.SucessoResposta(listaDto);
        }

        public async Task<RespostaApi<bool>> RemoverCategoria(string id, Guid usuarioId)
        {
            var categoria = await _repositorioCategorias.ObterPorIdAsync(id);
            if (categoria == null || categoria.UsuarioId != usuarioId)
                return RespostaApi<bool>.ErroResposta("Categoria não encontrada.");

            await _repositorioCategorias.RemoverAsync(id);
            return RespostaApi<bool>.SucessoResposta(true, "Categoria removida com sucesso.");
        }

        private static CategoriaDto MapearParaDto(Categoria categoria)
        {
            return new CategoriaDto
            {
                Id = categoria.Id,
                Nome = categoria.Nome,
                Tipo = categoria.Tipo,
                Descricao = categoria.Descricao,
                UsuarioId = categoria.UsuarioId,
                DataCriacao = categoria.DataCriacao,
                DataAtualizacao = categoria.DataAtualizacao ?? DateTime.UtcNow
            };
        }
    }
}
