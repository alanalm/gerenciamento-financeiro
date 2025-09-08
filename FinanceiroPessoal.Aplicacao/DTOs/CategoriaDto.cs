using FinanceiroPessoal.Dominio.Enums;

namespace FinanceiroPessoal.Aplicacao.DTOs
{
    public class CategoriaDto
    {
        public string Id { get; set; } = string.Empty;
        public Guid UsuarioId { get; set; } 
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public TipoCategoria Tipo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
    }

    public class CriarCategoriaDto
    {
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public TipoCategoria Tipo { get; set; }
    }

    public class AtualizarCategoriaDto
    {
        public string Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public TipoCategoria Tipo { get; set; }
    }
}
