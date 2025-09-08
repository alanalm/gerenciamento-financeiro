using FinanceiroPessoal.Aplicacao.DTOs;
using FinanceiroPessoal.Dominio.Comum;
using FinanceiroPessoal.Servicos.Api;
using MudBlazor;
using System.Collections.ObjectModel;

namespace FinanceiroPessoal.ViewModels
{
    public class CategoriaViewModel : BaseViewModel
    {
        private readonly ICategoriaApiService _categoriaService;
        private readonly ValidadorCriarCategorias _validadorCriar = new();
        private readonly ValidadorAtualizarCategorias _validadorAtualizar = new();

        public ObservableCollection<CategoriaDto> Categorias { get; private set; } = new ObservableCollection<CategoriaDto>();

        public string? MensagemErro { get; set; }

        public CategoriaViewModel(ICategoriaApiService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        public event Action<string, Severity>? OnSnackbar;

        /// <summary>
        /// Carrega todas as categorias do serviço
        /// </summary>
        public async Task CarregarCategoriasAsync()
        {
            await ExecutarOperacaoAsync(async () =>
            {
                var response = await _categoriaService.ObterTodosAsync();
                if (response.Sucesso && response.Dados != null)
                {
                    Categorias.Clear();
                    foreach (var cat in response.Dados)
                        Categorias.Add(cat);
                }
                else
                {
                    MensagemErro = response.Mensagem ?? "Não foi possível carregar categorias.";
                }
            });
        }

        /// <summary>
        /// Adiciona uma nova categoria
        /// </summary>
        public async Task<RespostaApi<CategoriaDto>>AdicionarCategoriaAsync(CriarCategoriaDto novaCategoria)
        {
            var validacao = ValidarDtoResultado(_validadorCriar, novaCategoria);

            if (!validacao.Sucesso)
                return RespostaApi<CategoriaDto>.ErroResposta(validacao.Mensagem);

            RespostaApi<CategoriaDto>? resultado = null;

            await ExecutarOperacaoAsync(async () =>
            {
                var response = await _categoriaService.AdicionarAsync(novaCategoria);
                if (response.Sucesso && response.Dados != null)
                {
                    Categorias.Add(response.Dados);
                    MensagemSucesso = "Categoria adicionada com sucesso!";
                }
                else
                {
                    MensagemErro = response.Mensagem ?? "Erro ao adicionar categoria.";
                }
            });
            return resultado ?? RespostaApi<CategoriaDto>.ErroResposta("Erro inesperado ao adicionar categoria.");
        }

        /// <summary>
        /// Atualiza uma categoria existente
        /// </summary>
        public async Task<RespostaApi<CategoriaDto>> AtualizarCategoriaAsync(string id, AtualizarCategoriaDto categoriaAtualizada)
        {
            var validacao = ValidarDtoResultado(_validadorAtualizar, categoriaAtualizada);

            if (!validacao.Sucesso)
                return RespostaApi<CategoriaDto>.ErroResposta(validacao.Mensagem);

            RespostaApi<CategoriaDto>? resultado = null;

            await ExecutarOperacaoAsync(async () =>
            {
                var response = await _categoriaService.AtualizarAsync(id, categoriaAtualizada);
                if (response.Sucesso && response.Dados != null)
                {
                    // Atualiza item na ObservableCollection
                    var index = Categorias.IndexOf(Categorias.First(c => c.Id == id));
                    Categorias[index] = response.Dados;
                    MensagemSucesso = "Categoria atualizada com sucesso!";
                }
                else
                {
                    MensagemErro = response.Mensagem ?? "Erro ao atualizar categoria.";
                }
            });
            return resultado ?? RespostaApi<CategoriaDto>.ErroResposta("Erro inesperado ao atualizar categoria.");
        }

        /// <summary>
        /// Remove uma categoria
        /// </summary>
        public async Task RemoverCategoriaAsync(string id)
        {
            await ExecutarOperacaoAsync(async () =>
            {
                var response = await _categoriaService.RemoverAsync(id);
                if (response.Sucesso)
                {
                    var categoria = Categorias.FirstOrDefault(c => c.Id == id);
                    if (categoria != null) Categorias.Remove(categoria);
                    MensagemSucesso = "Categoria removida com sucesso!";
                }
                else
                {
                    MensagemErro = response.Mensagem ?? "Erro ao remover categoria.";
                }
            });
        }
    }
}
