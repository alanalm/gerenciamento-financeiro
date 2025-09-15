namespace FinanceiroPessoal.Apresentacao.Servicos.Interfaces
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(string email, string senha);
        Task LogoutAsync();
        Task<bool> RegistrarAsync(string nome, string email, string senha);
    }
}
