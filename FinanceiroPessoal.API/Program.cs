using FluentValidation;
using FluentValidation.AspNetCore;
using FinanceiroPessoal.Aplicacao.Servicos;
using FinanceiroPessoal.Aplicacao.Validadores;
using FinanceiroPessoal.Dominio.Interfaces;
using FinanceiroPessoal.Infraestrutura.Repositorios;
using Microsoft.Azure.Cosmos;
using Microsoft.OpenApi.Models;
using FinanceiroPessoal.Aplicacao.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

// Configuração Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FinanceiroPessoal API", Version = "v1" });
});

// Configuração do Cosmos DB
builder.Services.AddSingleton((serviceProvider) => {
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var cosmosDbConnectionString = configuration["CosmosDb:ConnectionString"];
    var cosmosDbDatabaseName = configuration["CosmosDb:DatabaseName"];

    if (string.IsNullOrEmpty(cosmosDbConnectionString) || string.IsNullOrEmpty(cosmosDbDatabaseName))
    {
        throw new InvalidOperationException("CosmosDb:ConnectionString or CosmosDb:DatabaseName is not configured.");
    }

    var cosmosClient = new CosmosClient(cosmosDbConnectionString);
    cosmosClient.CreateDatabaseIfNotExistsAsync(cosmosDbDatabaseName).Wait();

    return cosmosClient;
});

// Configuração CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorDev",
        policy =>
        {
            policy.WithOrigins("https://localhost:7129")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Injeção de Dependência dos Repositórios
builder.Services.AddSingleton<IRepositorioReceitas>(serviceProvider =>
{
    var cosmosClient = serviceProvider.GetRequiredService<CosmosClient>();
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var databaseName = configuration["CosmosDb:DatabaseName"];
    var containerName = "Receitas";
    cosmosClient.GetDatabase(databaseName).CreateContainerIfNotExistsAsync(containerName, "/id").Wait();
    return new ReceitaRepositorio(cosmosClient, databaseName, containerName);
});

builder.Services.AddSingleton<IRepositorioDespesas>(serviceProvider =>
{
    var cosmosClient = serviceProvider.GetRequiredService<CosmosClient>();
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var databaseName = configuration["CosmosDb:DatabaseName"];
    var containerName = "Despesas";
    cosmosClient.GetDatabase(databaseName).CreateContainerIfNotExistsAsync(containerName, "/id").Wait();
    return new DespesaRepositorio(cosmosClient, databaseName, containerName);
});

builder.Services.AddSingleton<IRepositorioCategorias>(serviceProvider =>
{
    var cosmosClient = serviceProvider.GetRequiredService<CosmosClient>();
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var databaseName = configuration["CosmosDb:DatabaseName"];
    var containerName = "Categorias";
    cosmosClient.GetDatabase(databaseName).CreateContainerIfNotExistsAsync(containerName, "/id").Wait();
    return new CategoriaRepositorio(cosmosClient, databaseName, containerName);
});

// Injeção de Dependência dos Serviços
builder.Services.AddScoped<ICategoriaServico ,CategoriaServico>();
builder.Services.AddScoped<IReceitaServico ,ReceitaServico>();
builder.Services.AddScoped<IDespesaServico ,DespesaServico>();

//Configurar FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<ValidadorCriarDespesas>();
builder.Services.AddValidatorsFromAssemblyContaining<ValidadorAtualizarDespesas>();
builder.Services.AddValidatorsFromAssemblyContaining<ValidadorCriarReceitas>();
builder.Services.AddValidatorsFromAssemblyContaining<ValidadorAtualizarReceitas>();
builder.Services.AddValidatorsFromAssemblyContaining<ValidadorCriarCategorias>();
builder.Services.AddValidatorsFromAssemblyContaining<ValidadorAtualizarCategorias>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FinanceiroPessoal API V1");
    });
}

app.UseCors("AllowBlazorDev");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
