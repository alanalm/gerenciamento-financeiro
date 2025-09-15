using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceiroPessoal.Aplicacao.DTOs
{
    public class LoginRespostaDto
    {
        public string Token { get; set; } = null!;
        public string Nome { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}
