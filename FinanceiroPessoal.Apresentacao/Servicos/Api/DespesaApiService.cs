using FinanceiroPessoal.Aplicacao.DTOs;
using FinanceiroPessoal.Apresentacao.Servicos.Interfaces;
using FinanceiroPessoal.Dominio.Comum;
using FinanceiroPessoal.Servicos.Api;

namespace FinanceiroPessoal.Apresentacao.Servicos.Api
{
    public class DespesaApiService : ApiServiceBase, IDespesaApiService
    {
        private const string BaseUrl = "api/despesas";

        public DespesaApiService(HttpClient httpClient) : base(httpClient) { }

        public Task<RespostaApi<IEnumerable<DespesaDto>>> ObterTodosAsync()
            => GetAsync<IEnumerable<DespesaDto>>(BaseUrl);

        public Task<RespostaApi<DespesaDto>> ObterPorIdAsync(string id)
            => GetAsync<DespesaDto>($"{BaseUrl}/{id}");

        public Task<RespostaApi<DespesaDto>> AdicionarAsync(CriarDespesaDto despesa)
            => PostAsync<DespesaDto>(BaseUrl, despesa);

        public Task<RespostaApi<DespesaDto>> AtualizarAsync(string id, AtualizarDespesaDto despesa)
            => PutAsync<DespesaDto>($"{BaseUrl}/{id}", despesa);

        public Task<RespostaApi<bool>> RemoverAsync(string id)
            => DeleteAsync($"{BaseUrl}/{id}");
    }
}
