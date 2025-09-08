using FinanceiroPessoal.Dominio.Comum;

namespace FinanceiroPessoal.Dominio.Entidades
{
    public class Receita : EntidadeBase
    {
        public string Titulo { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public DateTime? DataRecebimento { get; set; }
        public string CategoriaId { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
    }
}
