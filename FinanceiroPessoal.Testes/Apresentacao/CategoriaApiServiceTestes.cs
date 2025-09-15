using Blazored.LocalStorage;
using FinanceiroPessoal.Aplicacao.DTOs;
using FinanceiroPessoal.Dominio.Comum;
using FinanceiroPessoal.Servicos.Api;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text;
using System.Text.Json;
using Xunit.Abstractions;

public class CategoriaApiServiceTestes
{
    private readonly ITestOutputHelper _saida;
    private readonly string BaseUrl = "https://localhost:5001/api/categorias";

    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;

    private readonly HttpClient _httpClient;

    private readonly CategoriaApiService _service;
    private readonly Mock<ILocalStorageService> localStorageMock = new();

    public CategoriaApiServiceTestes(ITestOutputHelper saida)
    {
        _saida = saida;
    }

    private static StringContent AsJson(object valor)
    {
        var json = JsonSerializer.Serialize(valor, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    [Fact]
    public async Task ObterTodasCategorias_DeveRetornarSucesso()
    {
        // Arrange: criamos o envelope que o ApiService espera
        var payload = RespostaApi<IEnumerable<CategoriaDto>>.SucessoResposta(new[]
        {
            new CategoriaDto { Id = "1", Nome = "Alimentação" },
            new CategoriaDto { Id = "2", Nome = "Transporte" }
        });

        // Mock do HttpMessageHandler para interceptar a chamada HTTP
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = AsJson(payload)
            })
            .Verifiable();

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("http://localhost/")
        };

        //localStorageMock = new Mock<ILocalStorageService>();
        localStorageMock
        .Setup(ls => ls.GetItemAsStringAsync("authToken", It.IsAny<CancellationToken>()))
        .ReturnsAsync("fake-jwt-token");

        var servico = new CategoriaApiService(httpClient, localStorageMock.Object);

        // Act
        var resultado = await servico.ObterTodosAsync();

        // Log para ajudar debug (vai aparecer no detalhe do teste no Test Explorer)
        _saida.WriteLine($"Sucesso: {resultado.Sucesso}");
        _saida.WriteLine($"Mensagem: {resultado.Mensagem}");
        _saida.WriteLine($"Itens retornados: {resultado.Dados?.Count()}");

        // Assert
        Assert.True(resultado.Sucesso, "Esperava que o retorno fosse sucesso.");
        Assert.NotNull(resultado.Dados);
        Assert.Equal(2, resultado.Dados.Count());
        Assert.Contains(resultado.Dados, c => c.Nome == "Alimentação");

        // Verifica que o handler foi chamado
        handlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
    }

    private CategoriaApiService CriarServicoComResposta(object resposta, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var json = JsonSerializer.Serialize(new
        {
            sucesso = statusCode == HttpStatusCode.OK,
            mensagem = statusCode == HttpStatusCode.OK ? "Operação realizada com sucesso" : "Erro",
            dados = resposta
        });

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("http://localhost")
        };

        return new CategoriaApiService(httpClient, localStorageMock.Object);
    }

    [Fact(DisplayName = "Criar categoria deve retornar sucesso quando criada")]
    public async Task CriarAsync_DeveRetornarSucesso_QuandoCriada()
    {
        var categoriaCriada = new CategoriaDto { Id = "1", Nome = "Categoria Teste" };
        var service = CriarServicoComResposta(categoriaCriada);

        var resultado = await service.AdicionarAsync(new CriarCategoriaDto { Nome = "Categoria Teste" });

        Assert.True(resultado.Sucesso, "Esperava que a criação retornasse sucesso.");
        Assert.NotNull(resultado.Dados);
        Assert.Equal("Categoria Teste", resultado.Dados.Nome);
    }

    [Fact(DisplayName = "Atualizar categoria deve retornar sucesso quando atualizada")]
    public async Task AtualizarAsync_DeveRetornarSucesso_QuandoAtualizada()
    {
        var categoriaAtualizada = new CategoriaDto { Id = "1", Nome = "Categoria Atualizada" };
        var service = CriarServicoComResposta(categoriaAtualizada);

        var resultado = await service.AtualizarAsync("1", new AtualizarCategoriaDto { Nome = "Categoria Atualizada" });

        Assert.True(resultado.Sucesso, "Esperava que a atualização retornasse sucesso.");
        Assert.NotNull(resultado.Dados);
        Assert.Equal("Categoria Atualizada", resultado.Dados.Nome);
    }

    [Fact(DisplayName = "Obter categoria por Id deve retornar sucesso")]
    public async Task ObterPorIdAsync_DeveRetornarSucesso()
    {
        var categoria = new CategoriaDto { Id = "1", Nome = "Categoria Teste" };
        var service = CriarServicoComResposta(categoria);

        var resultado = await service.ObterPorIdAsync("1");

        Assert.True(resultado.Sucesso, "Esperava que a busca por Id retornasse sucesso.");
        Assert.NotNull(resultado.Dados);
        Assert.Equal("1", resultado.Dados.Id);
    }

    [Fact(DisplayName = "Remover categoria deve retornar sucesso quando removida")]
    public async Task RemoverAsync_DeveRetornarSucesso_QuandoRemovida()
    {
        var service = CriarServicoComResposta(true);

        var resultado = await service.RemoverAsync("1");

        Assert.True(resultado.Sucesso, "Esperava que a remoção retornasse sucesso.");
        Assert.True(resultado.Dados, "Esperava que o retorno indicasse que foi removida.");
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarFalha_QuandoNaoEncontrado()
    {
        // Arrange
        var categoriaId = "999"; // inexistente
        var respostaApi = new HttpResponseMessage(HttpStatusCode.NotFound);

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri!.AbsolutePath == $"/api/categorias/{categoriaId}"),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(respostaApi);

        var httpClient = new HttpClient(handlerMock.Object) { BaseAddress = new Uri("http://localhost") };
        var service = new CategoriaApiService(httpClient, localStorageMock.Object);

        // Act
        var resultado = await service.ObterPorIdAsync(categoriaId);

        // Assert
        Assert.False(resultado.Sucesso);
        Assert.Null(resultado.Dados);
        Assert.Contains("Erro da API", resultado.Mensagem, StringComparison.OrdinalIgnoreCase);
    }


    [Fact]
    public async Task CriarAsync_DeveRetornarFalha_QuandoDadosInvalidos()
    {
        // Arrange
        var novaCategoria = new CriarCategoriaDto { Nome = "" }; // inválido
        var respostaApi = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("{\"mensagem\":\"Nome inválido\"}")
        };

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri!.AbsolutePath == "/api/categorias"),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(respostaApi);

        var httpClient = new HttpClient(handlerMock.Object) { BaseAddress = new Uri("http://localhost") };
        var service = new CategoriaApiService(httpClient, localStorageMock.Object);

        // Act
        var resultado = await service.AdicionarAsync(novaCategoria);

        // Assert
        Assert.False(resultado.Sucesso);
        Assert.Null(resultado.Dados);
        Assert.Contains("inválido", resultado.Mensagem, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AtualizarAsync_DeveRetornarFalha_QuandoNaoEncontrado()
    {
        // Arrange
        var categoriaId = "999"; // inexistente
        var atualizarDto = new AtualizarCategoriaDto { Nome = "Nova Categoria" };

        var respostaApi = new HttpResponseMessage(HttpStatusCode.NotFound);

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Put &&
                    req.RequestUri!.AbsolutePath == $"/api/categorias/{categoriaId}"),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(respostaApi);

        var httpClient = new HttpClient(handlerMock.Object) { BaseAddress = new Uri("http://localhost") };
        var service = new CategoriaApiService(httpClient, localStorageMock.Object);

        // Act
        var resultado = await service.AtualizarAsync(categoriaId, atualizarDto);

        // Assert
        Assert.False(resultado.Sucesso);
        Assert.Contains("Erro da API", resultado.Mensagem, StringComparison.OrdinalIgnoreCase);
    }



    [Fact]
    public async Task RemoverAsync_DeveRetornarFalha_QuandoErroServidor()
    {
        // Arrange
        var categoriaId = "1";
        var respostaApi = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("{\"mensagem\":\"Erro interno\"}")
        };

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Delete &&
                    req.RequestUri!.AbsolutePath == $"/api/categorias/{categoriaId}"),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(respostaApi);

        var httpClient = new HttpClient(handlerMock.Object) { BaseAddress = new Uri("http://localhost") };
        var service = new CategoriaApiService(httpClient, localStorageMock.Object);

        // Act
        var resultado = await service.RemoverAsync(categoriaId);

        // Assert
        Assert.False(resultado.Sucesso);
        Assert.Contains("erro", resultado.Mensagem, StringComparison.OrdinalIgnoreCase);
    }
}
