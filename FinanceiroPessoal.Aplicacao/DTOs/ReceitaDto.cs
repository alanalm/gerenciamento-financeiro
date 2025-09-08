using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceiroPessoal.Aplicacao.DTOs
{
    public class ReceitaDto
    {
        public string Id { get; set; } = string.Empty;
        public Guid UsuarioId { get; set; }
        public decimal Valor { get; set; }
        public DateTime? Data { get; set; }
        public string CategoriaId { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
    }

    public class CriarReceitaDto
    {
        public decimal Valor { get; set; }
        public DateTime? Data { get; set; }
        public string CategoriaId { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
    }

    public class AtualizarReceitaDto
    {
        public string Id { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public DateTime? Data { get; set; }
        public string CategoriaId { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
    }
}
