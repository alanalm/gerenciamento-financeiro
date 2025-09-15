using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceiroPessoal.Aplicacao.DTOs
{
    public class UsuarioDto
    {
        public string Id { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Nome { get; set; }
        public string Role { get; set; } = "Usuario";
    }
}
