using FinanceiroPessoal.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceiroPessoal.Dominio.Interfaces
{
    public interface IRepositorioCategorias : IRepositorioBase<Categoria>
    {
        Task<IEnumerable<Categoria>> ObterPorUsuarioAsync(Guid usuarioId);
        Task<IEnumerable<Categoria>> ObterPorTipoAsync(Guid usuarioId, Enums.TipoCategoria tipo);
        Task<Categoria?> ObterPorIdAsync(string id, Guid usuarioId);
        Task RemoverAsync(string id, Guid usuarioId);
    }
}
