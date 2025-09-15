using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FinanceiroPessoal.Apresentacao;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly ClaimsPrincipal _usuarioAnonimo = new(new ClaimsIdentity());

    public CustomAuthStateProvider(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");

        if (string.IsNullOrEmpty(token))
            return new AuthenticationState(_usuarioAnonimo);

        try
        {
            var identity = ObterClaimsDoToken(token);
            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }
        catch
        {
            // Se o token for inválido, remove ele e volta para anônimo
            await _localStorage.RemoveItemAsync("authToken");
            return new AuthenticationState(_usuarioAnonimo);
        }
    }

    public async Task NotifyUserAuthenticationAsync(string token)
    {
        try
        {
            await _localStorage.SetItemAsync("authToken", token);

            var identity = ObterClaimsDoToken(token);
            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }
        catch
        {
            await _localStorage.RemoveItemAsync("authToken");
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_usuarioAnonimo)));
        }
    }

    public async Task NotifyUserLogoutAsync()
    {
        await _localStorage.RemoveItemAsync("authToken");
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_usuarioAnonimo)));
    }

    private ClaimsIdentity ObterClaimsDoToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        var claims = jwt.Claims.ToList();

        return new ClaimsIdentity(claims, "Bearer");
    }
}
