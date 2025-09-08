using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FinanceiroPessoal.Dominio.Interfaces
{
    public interface IRepositorioBase<T>
    {
        Task<T> AdicionarAsync(T entidade);
        Task<T> ObterPorIdAsync(string id);
        Task<IEnumerable<T>> ObterTodosAsync();
        Task<IEnumerable<T>> BuscarAsync(Expression<Func<T, bool>> predicate);
        Task AtualizarAsync(T entidade);
        Task RemoverAsync(string id);
    }
}
