using Blazored.LocalStorage;
using FinanceiroPessoal.Aplicacao.DTOs;
using FinanceiroPessoal.Dominio.Comum;

namespace FinanceiroPessoal.Servicos.Api
{
    public class CategoriaApiService : ApiServiceBase, ICategoriaApiService
    {
        private const string BaseUrl = "categorias";

        public CategoriaApiService(HttpClient httpClient, ILocalStorageService localStorage)
            : base(httpClient, localStorage)
        {
        }

        public Task<RespostaApi<IEnumerable<CategoriaDto>>> ObterTodosAsync()
            => GetAsync<IEnumerable<CategoriaDto>>(BaseUrl);

        public Task<RespostaApi<CategoriaDto>> ObterPorIdAsync(string id)
            => GetAsync<CategoriaDto>($"{BaseUrl}/{id}");

        public Task<RespostaApi<CategoriaDto>> AdicionarAsync(CriarCategoriaDto categoria)
            => PostAsync<CategoriaDto>(BaseUrl, categoria);

        public Task<RespostaApi<CategoriaDto>> AtualizarAsync(string id, AtualizarCategoriaDto categoria)
            => PutAsync<CategoriaDto>($"{BaseUrl}/{id}", categoria);

        public Task<RespostaApi<bool>> RemoverAsync(string id)
            => DeleteAsync($"{BaseUrl}/{id}");
    }
}
