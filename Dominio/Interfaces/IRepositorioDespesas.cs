using FinanceiroPessoal.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceiroPessoal.Dominio.Interfaces
{
    public interface IRepositorioDespesas : IRepositorioBase<Despesa>
    {
        Task<IEnumerable<Despesa>> ObterPorUsuarioAsync(Guid usuarioId);
        Task<Despesa?> ObterPorIdAsync(string id, Guid usuarioId);
        Task RemoverAsync(string id, Guid usuarioId);
    }
}
