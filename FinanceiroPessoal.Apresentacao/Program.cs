using Blazored.LocalStorage;
using FinanceiroPessoal.Apresentacao;
using FinanceiroPessoal.Apresentacao.Handlers;
using FinanceiroPessoal.Apresentacao.Servicos;
using FinanceiroPessoal.Apresentacao.Servicos.Api;
using FinanceiroPessoal.Apresentacao.Servicos.Interfaces;
using FinanceiroPessoal.Apresentacao.ViewModels;
using FinanceiroPessoal.Servicos.Api;
using FinanceiroPessoal.ViewModels;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;
using MudBlazor.Translations;
using System.Net.Http.Json;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddBlazoredLocalStorage();

        // Cria HttpClient temporário para ler o appsettings.json
        var tempClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
        var settings = await tempClient.GetFromJsonAsync<Dictionary<string, string>>("appsettings.json");

        // Lê a URL base da API ou usa padrão
        var apiUrl = settings?["ApiUrl"] ?? "https://localhost:7128/";

        // Registra o handler que injeta o token
        builder.Services.AddTransient<AuthHeaderHandler>();

        // HttpClient da API com AuthHeaderHandler
        builder.Services.AddHttpClient("API", client =>
        {
            client.BaseAddress = new Uri(apiUrl + "api/");
        }).AddHttpMessageHandler<AuthHeaderHandler>();

        // HttpClient padrão injetado nos serviços
        builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));

        builder.Services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;
            config.SnackbarConfiguration.PreventDuplicates = true;
            config.SnackbarConfiguration.NewestOnTop = true;
            config.SnackbarConfiguration.ShowCloseIcon = true;
            config.SnackbarConfiguration.VisibleStateDuration = 4000;
        });

        builder.Services.AddMudTranslations()
            .AddMudLocalization();

        // Serviços da API
        builder.Services.AddScoped<ICategoriaApiService, CategoriaApiService>();
        builder.Services.AddScoped<IReceitaApiService, ReceitaApiService>();
        builder.Services.AddScoped<IDespesaApiService, DespesaApiService>();
        builder.Services.AddScoped<IAuthService, AuthService>();

        //ViewModels
        builder.Services.AddScoped<CategoriaViewModel>();
        builder.Services.AddScoped<ReceitaViewModel>();
        builder.Services.AddScoped<DespesaViewModel>();
        builder.Services.AddScoped<DashboardViewModel>();

        // Autenticação e Autorização
        builder.Services.AddScoped<CustomAuthStateProvider>();
        builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>());
        builder.Services.AddAuthorizationCore();

        await builder.Build().RunAsync();
    }
}