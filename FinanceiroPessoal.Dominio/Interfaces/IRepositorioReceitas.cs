using FinanceiroPessoal.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceiroPessoal.Dominio.Interfaces
{
    public interface IRepositorioReceitas : IRepositorioBase<Receita>
    {
        Task<IEnumerable<Receita>> ObterPorUsuarioAsync(Guid usuarioId);
        Task<Receita?> ObterPorIdAsync(string id, Guid usuarioId);
        Task<IEnumerable<Receita>> ObterPorPeriodoAsync(Guid usuarioId, DateTime inicio, DateTime fim);
        Task RemoverAsync(string id, Guid usuarioId);
    }
}
