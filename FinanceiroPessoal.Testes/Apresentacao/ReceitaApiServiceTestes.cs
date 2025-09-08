using FinanceiroPessoal.Aplicacao.DTOs;
using FinanceiroPessoal.Apresentacao.Servicos.Api;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;

namespace SeuProjeto.Tests.Servicos
{
    public class ReceitaApiServiceTestes
    {
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        private HttpClient CriarHttpClient(HttpStatusCode statusCode, object? dados = null, bool sucesso = true, string mensagem = "ok")
        {
            var handlerMock = new Mock<HttpMessageHandler>();

            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(() =>
                {
                    HttpContent? conteudo = null;

                    if (dados != null || statusCode == HttpStatusCode.OK)
                    {
                        var envelope = new
                        {
                            sucesso,
                            mensagem,
                            dados
                        };

                        conteudo = new StringContent(JsonSerializer.Serialize(envelope, _jsonOptions), Encoding.UTF8, "application/json");
                    }
                    else if (!sucesso)
                    {
                        conteudo = new StringContent(mensagem, Encoding.UTF8, "text/plain");
                    }

                    return new HttpResponseMessage
                    {
                        StatusCode = statusCode,
                        Content = conteudo
                    };
                });

            return new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };
        }

        [Fact]
        public async Task ObterTodosAsync_DeveRetornarSucesso_QuandoApiResponderOk()
        {
            var receitas = new List<ReceitaDto>
            {
                new() { Id = "1", Descricao = "Receita 1", Valor = 100 },
                new() { Id = "2", Descricao = "Receita 2", Valor = 200 }
            };

            var httpClient = CriarHttpClient(HttpStatusCode.OK, receitas);
            var service = new ReceitaApiService(httpClient);

            var resultado = await service.ObterTodosAsync();

            Assert.True(resultado.Sucesso);
            Assert.Equal(2, resultado.Dados!.Count());
        }

        [Fact]
        public async Task ObterTodosAsync_DeveRetornarFalha_QuandoApiRetornarErro()
        {
            var httpClient = CriarHttpClient(HttpStatusCode.InternalServerError, null, false, "erro interno");
            var service = new ReceitaApiService(httpClient);

            var resultado = await service.ObterTodosAsync();

            Assert.False(resultado.Sucesso);
            Assert.Null(resultado.Dados);
        }

        [Fact]
        public async Task ObterPorIdAsync_DeveRetornarSucesso_QuandoEncontrado()
        {
            var receita = new ReceitaDto { Id = "1", Descricao = "Receita Teste", Valor = 150 };
            var httpClient = CriarHttpClient(HttpStatusCode.OK, receita);
            var service = new ReceitaApiService(httpClient);

            var resultado = await service.ObterPorIdAsync("1");

            Assert.True(resultado.Sucesso);
            Assert.NotNull(resultado.Dados);
            Assert.Equal("Receita Teste", resultado.Dados!.Descricao);
        }

        [Fact]
        public async Task ObterPorIdAsync_DeveRetornarFalha_QuandoNaoEncontrado()
        {
            var httpClient = CriarHttpClient(HttpStatusCode.NotFound, null, false, "não encontrado");
            var service = new ReceitaApiService(httpClient);

            var resultado = await service.ObterPorIdAsync("99");

            Assert.False(resultado.Sucesso);
            Assert.Null(resultado.Dados);
            Assert.Contains("não encontrado", resultado.Mensagem, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task CriarAsync_DeveRetornarSucesso_QuandoCriado()
        {
            var novaReceita = new CriarReceitaDto { Descricao = "Nova Receita", Valor = 300 };
            var receitaCriada = new ReceitaDto { Id = "10", Descricao = "Nova Receita", Valor = 300 };

            var httpClient = CriarHttpClient(HttpStatusCode.OK, receitaCriada);
            var service = new ReceitaApiService(httpClient);

            var resultado = await service.AdicionarAsync(novaReceita);

            Assert.True(resultado.Sucesso);
            Assert.NotNull(resultado.Dados);
            Assert.Equal("10", resultado.Dados!.Id);
        }

        [Fact]
        public async Task CriarAsync_DeveRetornarFalha_QuandoErroNaApi()
        {
            var novaReceita = new CriarReceitaDto { Descricao = "Falha", Valor = 999 };
            var httpClient = CriarHttpClient(HttpStatusCode.BadRequest, null, false, "erro ao criar");
            var service = new ReceitaApiService(httpClient);

            var resultado = await service.AdicionarAsync(novaReceita);

            Assert.False(resultado.Sucesso);
            Assert.Null(resultado.Dados);
        }

        [Fact]
        public async Task AtualizarAsync_DeveRetornarSucesso_QuandoAtualizado()
        {
            var atualizarReceita = new AtualizarReceitaDto { CategoriaId = "1", Descricao = "Receita Atualizada", Valor = 500 };
            var receitaAtualizada = new ReceitaDto { Id = "1", Descricao = "Receita Atualizada", Valor = 500 };

            var httpClient = CriarHttpClient(HttpStatusCode.OK, receitaAtualizada);
            var service = new ReceitaApiService(httpClient);

            var resultado = await service.AtualizarAsync("1", atualizarReceita);

            Assert.True(resultado.Sucesso);
            Assert.Equal("Receita Atualizada", resultado.Dados!.Descricao);
        }

        [Fact]
        public async Task AtualizarAsync_DeveRetornarFalha_QuandoNaoEncontrado()
        {
            var atualizarReceita = new AtualizarReceitaDto { CategoriaId = "999", Descricao = "Inexistente", Valor = 100 };
            var httpClient = CriarHttpClient(HttpStatusCode.NotFound, null, false, "não encontrado");
            var service = new ReceitaApiService(httpClient);

            var resultado = await service.AtualizarAsync("999", atualizarReceita);

            Assert.False(resultado.Sucesso);
            Assert.Null(resultado.Dados);
            Assert.Contains("não encontrado", resultado.Mensagem, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task RemoverAsync_DeveRetornarSucesso_QuandoRemovido()
        {
            var httpClient = CriarHttpClient(HttpStatusCode.OK, true);
            var service = new ReceitaApiService(httpClient);

            var resultado = await service.RemoverAsync("1");

            Assert.True(resultado.Sucesso);
            Assert.True(resultado.Dados);
        }

        [Fact]
        public async Task RemoverAsync_DeveRetornarFalha_QuandoNaoEncontrado()
        {
            var httpClient = CriarHttpClient(HttpStatusCode.NotFound, null, false, "não encontrado");
            var service = new ReceitaApiService(httpClient);

            var resultado = await service.RemoverAsync("99");

            Assert.False(resultado.Sucesso);
            Assert.Contains("não encontrado", resultado.Mensagem, StringComparison.OrdinalIgnoreCase);
        }
    }
}
