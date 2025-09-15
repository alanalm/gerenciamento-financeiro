using FinanceiroPessoal.Dominio.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceiroPessoal.Dominio.Entidades
{
    public class Usuario : EntidadeBase
    {
        public string Email { get; set; } = null!;
        public string SenhaHash { get; set; } = null!; 
        public string? Nome { get; set; } = null;
        public string Role { get; set; } = "Usuario";

        public Usuario() { }

        public Usuario(string email, string senhaHash)
        {
            Email = email;
            SenhaHash = senhaHash;
            Nome = string.Empty;
            Role = "Usuario";
        }
    }
}
