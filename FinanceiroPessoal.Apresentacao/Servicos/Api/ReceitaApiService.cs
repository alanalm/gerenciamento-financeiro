using FinanceiroPessoal.Aplicacao.DTOs;
using FinanceiroPessoal.Apresentacao.Servicos.Interfaces;
using FinanceiroPessoal.Dominio.Comum;
using FinanceiroPessoal.Servicos.Api;

namespace FinanceiroPessoal.Apresentacao.Servicos.Api
{
    public class ReceitaApiService : ApiServiceBase, IReceitaApiService
    {
        private const string BaseUrl = "api/receitas";

        public ReceitaApiService(HttpClient httpClient) : base(httpClient) { }

        public Task<RespostaApi<IEnumerable<ReceitaDto>>> ObterTodosAsync()
            => GetAsync<IEnumerable<ReceitaDto>>(BaseUrl);

        public Task<RespostaApi<ReceitaDto>> ObterPorIdAsync(string id)
            => GetAsync<ReceitaDto>($"{BaseUrl}/{id}");

        public Task<RespostaApi<ReceitaDto>> AdicionarAsync(CriarReceitaDto receita)
            => PostAsync<ReceitaDto>(BaseUrl, receita);

        public Task<RespostaApi<ReceitaDto>> AtualizarAsync(string id, AtualizarReceitaDto receita)
            => PutAsync<ReceitaDto>($"{BaseUrl}/{id}", receita);

        public Task<RespostaApi<bool>> RemoverAsync(string id)
            => DeleteAsync($"{BaseUrl}/{id}");
    }
}
