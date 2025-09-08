using FinanceiroPessoal.Aplicacao.DTOs;
using FinanceiroPessoal.Dominio.Comum;

namespace FinanceiroPessoal.Apresentacao.Servicos.Interfaces
{
    public interface IDespesaApiService
    {
        Task<RespostaApi<IEnumerable<DespesaDto>>> ObterTodosAsync();
        Task<RespostaApi<DespesaDto>> ObterPorIdAsync(string id);
        Task<RespostaApi<DespesaDto>> AdicionarAsync(CriarDespesaDto despesa);
        Task<RespostaApi<DespesaDto>> AtualizarAsync(string id, AtualizarDespesaDto despesa);
        Task<RespostaApi<bool>> RemoverAsync(string id);
    }
}
