using FinanceiroPessoal.Aplicacao.DTOs;
using FinanceiroPessoal.Dominio.Comum;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceiroPessoal.Servicos.Api
{
    public interface ICategoriaApiService
    {
        Task<RespostaApi<IEnumerable<CategoriaDto>>> ObterTodosAsync();
        Task<RespostaApi<CategoriaDto>> ObterPorIdAsync(string id);
        Task<RespostaApi<CategoriaDto>> AdicionarAsync(CriarCategoriaDto categoria);
        Task<RespostaApi<CategoriaDto>> AtualizarAsync(string id, AtualizarCategoriaDto categoria);
        Task<RespostaApi<bool>> RemoverAsync(string id);
    }
}
