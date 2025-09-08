using FinanceiroPessoal.Dominio.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceiroPessoal.Dominio.Entidades
{
    public class Despesa : EntidadeBase
    {
        public string Titulo { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public DateTime? DataPagamento { get; set; }
        public string CategoriaId { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public bool Pago { get; set; }
    }
}
