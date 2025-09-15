using FinanceiroPessoal.Aplicacao.DTOs;
using FinanceiroPessoal.Dominio.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceiroPessoal.Aplicacao.Interfaces
{
    public interface IAuthServico
    {
        Task<RespostaApi<LoginRespostaDto>> LoginAsync(string email, string senha);
        Task<RespostaApi<UsuarioDto>> RegistrarAsync(UsuarioDto usuarioDto, string senha);
    }
}
