using FinanceiroPessoal.Aplicacao.Interfaces;
using FinanceiroPessoal.Aplicacao.Servicos;
using FinanceiroPessoal.Aplicacao.Validadores;
using FinanceiroPessoal.Dominio.Entidades;
using FinanceiroPessoal.Dominio.Interfaces;
using FinanceiroPessoal.Infraestrutura.Repositorios;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Azure.Cosmos;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// JWT Configuração
var jwtSection = builder.Configuration.GetSection("Jwt");
var key = jwtSection["Key"];
var issuer = jwtSection["Issuer"];
var audience = jwtSection["Audience"];

// Adicionar serviços ao contêiner.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

// Configuração Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FinanceiroPessoal API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT assim: Bearer {seu token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
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
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
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

builder.Services.AddSingleton<IUsuarioRepositorio>(serviceProvider =>
{
    var cosmosClient = serviceProvider.GetRequiredService<CosmosClient>();
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var databaseName = configuration["CosmosDb:DatabaseName"];
    var containerName = "Usuarios";
    cosmosClient.GetDatabase(databaseName).CreateContainerIfNotExistsAsync(containerName, "/id").Wait();
    return new UsuarioRepositorio(cosmosClient, databaseName, containerName);
});

// Injeção de Dependência dos Serviços
builder.Services.AddScoped<ICategoriaServico ,CategoriaServico>();
builder.Services.AddScoped<IReceitaServico ,ReceitaServico>();
builder.Services.AddScoped<IDespesaServico ,DespesaServico>();
builder.Services.AddScoped<IAuthServico , AuthServico>();
builder.Services.AddScoped<ITokenGerador ,TokenGerador>();

//Configurar FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<ValidadorCriarDespesas>();
builder.Services.AddValidatorsFromAssemblyContaining<ValidadorAtualizarDespesas>();
builder.Services.AddValidatorsFromAssemblyContaining<ValidadorCriarReceitas>();
builder.Services.AddValidatorsFromAssemblyContaining<ValidadorAtualizarReceitas>();
builder.Services.AddValidatorsFromAssemblyContaining<ValidadorCriarCategorias>();
builder.Services.AddValidatorsFromAssemblyContaining<ValidadorAtualizarCategorias>();

builder.Services.AddScoped<IAuthServico, AuthServico>();

// JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!))
        };
    });

var app = builder.Build();

// configurar o pipeline HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FinanceiroPessoal API V1");
    });
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
