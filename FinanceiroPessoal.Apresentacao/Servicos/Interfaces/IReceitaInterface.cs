using FinanceiroPessoal.Aplicacao.DTOs;
using FinanceiroPessoal.Dominio.Comum;

namespace FinanceiroPessoal.Apresentacao.Servicos.Interfaces
{
    public interface IReceitaApiService
    {
        Task<RespostaApi<IEnumerable<ReceitaDto>>> ObterTodosAsync();
        Task<RespostaApi<ReceitaDto>> ObterPorIdAsync(string id);
        Task<RespostaApi<ReceitaDto>> AdicionarAsync(CriarReceitaDto receita);
        Task<RespostaApi<ReceitaDto>> AtualizarAsync(string id, AtualizarReceitaDto receita);
        Task<RespostaApi<bool>> RemoverAsync(string id);
    }

}
