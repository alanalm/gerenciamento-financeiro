using FinanceiroPessoal.Dominio.Comum;
using FinanceiroPessoal.Dominio.Enums;

namespace FinanceiroPessoal.Dominio.Entidades
{
    public class Categoria : EntidadeBase
    {
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public TipoCategoria Tipo { get; set; }
    }
}
