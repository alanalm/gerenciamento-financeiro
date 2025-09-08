using FinanceiroPessoal.Aplicacao.DTOs;
using FinanceiroPessoal.Dominio.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceiroPessoal.Aplicacao.Interfaces
{
    public interface IReceitaServico
    {
        Task<RespostaApi<ReceitaDto>> AdicionarReceita(CriarReceitaDto receitaDto, Guid usuarioId);
        Task<RespostaApi<ReceitaDto>> ObterReceitaPorId(string id, Guid usuarioId);
        Task<RespostaApi<IEnumerable<ReceitaDto>>> ObterReceitasPorUsuario(Guid usuarioId);
        Task<RespostaApi<ReceitaDto>> AtualizarReceita(string id, AtualizarReceitaDto receitaDto, Guid usuarioId);
        Task<RespostaApi<bool>> RemoverReceita(string id, Guid usuarioId);
    }
}
