namespace FinanceiroPessoal.Aplicacao.DTOs
{
    public class RegistrarUsuarioDto
    {
        public string Email { get; set; } = null!;
        public string Senha { get; set; } = null!;
        public string? Nome { get; set; }
    }
}
