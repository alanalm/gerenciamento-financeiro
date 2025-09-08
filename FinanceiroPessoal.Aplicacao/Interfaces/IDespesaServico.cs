using FinanceiroPessoal.Aplicacao.DTOs;
using FinanceiroPessoal.Dominio.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceiroPessoal.Aplicacao.Interfaces
{
    public interface IDespesaServico
    {
        Task<RespostaApi<DespesaDto>> AdicionarDespesa(CriarDespesaDto despesaDto, Guid usuarioId);
        Task<RespostaApi<DespesaDto>> ObterDespesaPorId(string id, Guid usuarioId);
        Task<RespostaApi<IEnumerable<DespesaDto>>> ObterDespesasPorUsuario(Guid usuarioId);
        Task<RespostaApi<DespesaDto>> AtualizarDespesa(string id, AtualizarDespesaDto despesaDto, Guid usuarioId);
        Task<RespostaApi<bool>> RemoverDespesa(string id, Guid usuarioId);
    }
}
