using FinanceiroPessoal.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceiroPessoal.Dominio.Interfaces
{
    public interface IUsuarioRepositorio : IRepositorioBase<Usuario>
    {
        Task<Usuario?> ObterPorEmailAsync(string email);
        Task<Usuario?> ObterPorIdAsync(string id);
        Task CriarAsync(Usuario usuario);
    }
}
