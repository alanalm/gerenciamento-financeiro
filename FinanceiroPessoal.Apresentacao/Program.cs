using FinanceiroPessoal.Apresentacao;
using FinanceiroPessoal.Apresentacao.Servicos.Api;
using FinanceiroPessoal.Apresentacao.Servicos.Interfaces;
using FinanceiroPessoal.Apresentacao.ViewModels;
using FinanceiroPessoal.Servicos.Api;
using FinanceiroPessoal.ViewModels;
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

        // Cria HttpClient tempor�rio para ler o appsettings.json
        var tempClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
        var settings = await tempClient.GetFromJsonAsync<Dictionary<string, string>>("appsettings.json");

        // L� a URL base da API ou usa padr�o
        var apiUrl = settings?["ApiUrl"] ?? "https://localhost:7128/";

        // Registra HttpClient definitivo
        builder.Services.AddScoped(sp => new HttpClient
        {
            BaseAddress = new Uri(apiUrl)
        });

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

        // Servi�os
        builder.Services.AddScoped<ICategoriaApiService, CategoriaApiService>();
        builder.Services.AddScoped<IReceitaApiService, ReceitaApiService>();
        builder.Services.AddScoped<IDespesaApiService, DespesaApiService>();

        //ViewModels
        builder.Services.AddScoped<CategoriaViewModel>();
        builder.Services.AddScoped<ReceitaViewModel>();
        builder.Services.AddScoped<DespesaViewModel>();
        builder.Services.AddScoped<DashboardViewModel>();

        await builder.Build().RunAsync();
    }
}