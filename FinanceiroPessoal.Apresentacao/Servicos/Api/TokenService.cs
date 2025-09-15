using FinanceiroPessoal.Apresentacao.Servicos.Interfaces;

namespace FinanceiroPessoal.Apresentacao.Servicos.Api
{
    public class TokenService : ITokenService
    {
        public string? Token { get; set; }
    }
}
