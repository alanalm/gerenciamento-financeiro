﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceiroPessoal.Aplicacao.DTOs
{
    public class LoginDto
    {
        public string Email { get; set; } = null!;
        public string Senha { get; set; } = null!;
    }
}
