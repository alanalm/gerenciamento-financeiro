using Blazored.LocalStorage;
using FinanceiroPessoal.Aplicacao.DTOs;
using FinanceiroPessoal.Apresentacao.Servicos.Interfaces;
using FinanceiroPessoal.Dominio.Comum;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FinanceiroPessoal.Apresentacao.Servicos;

public class AuthService : IAuthService
{
    private readonly HttpClient _http;
    private readonly ILocalStorageService _localStorage;
    private readonly AuthenticationStateProvider _authStateProvider;

    public AuthService(HttpClient http,
                       ILocalStorageService localStorage,
                       AuthenticationStateProvider authStateProvider)
    {
        _http = http;
        _localStorage = localStorage;
        _authStateProvider = authStateProvider;
    }

    public async Task<string?> LoginAsync(string email, string senha)
    {
        var response = await _http.PostAsJsonAsync("auth/login", new { Email = email, Senha = senha });

        if (!response.IsSuccessStatusCode)
            return null;

        var resultado = await response.Content.ReadFromJsonAsync<RespostaApi<LoginRespostaDto>>();

        if (resultado == null || !resultado.Sucesso || string.IsNullOrEmpty(resultado.Dados?.Token))
            return null;

        var token = resultado.Dados.Token;

        // salva também o nome
        if (!string.IsNullOrEmpty(resultado.Dados.Nome))
            await _localStorage.SetItemAsync("userName", resultado.Dados.Nome);

        // Salva o token no LocalStorage
        await _localStorage.SetItemAsync("authToken", token);

        // Notifica o CustomAuthStateProvider
        await ((CustomAuthStateProvider)_authStateProvider).NotifyUserAuthenticationAsync(token);

        // Configura o HttpClient para enviar o token nas próximas requisições
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return token;
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync("authToken");

        await ((CustomAuthStateProvider)_authStateProvider).NotifyUserLogoutAsync();

        _http.DefaultRequestHeaders.Authorization = null;
    }

    public async Task<string?> ObterTokenAsync()
    {
        return await _localStorage.GetItemAsync<string>("authToken");
    }

    public async Task<bool> RegistrarAsync(string nome, string email, string senha)
    {
        var response = await _http.PostAsJsonAsync("auth/registrar", new { Nome = nome, Email = email, Senha = senha });

        if (!response.IsSuccessStatusCode)
            return false;

        var resultado = await response.Content.ReadFromJsonAsync<RespostaApi<object>>();

        return resultado != null && resultado.Sucesso;
    }
}
