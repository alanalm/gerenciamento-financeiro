using FinanceiroPessoal.Aplicacao.DTOs;
using FinanceiroPessoal.Aplicacao.Servicos;
using FinanceiroPessoal.Dominio.Entidades;
using FinanceiroPessoal.Dominio.Interfaces;
using Moq;
using Xunit;

public class AuthServicoTests
{
    [Fact]
    public async Task LoginAsync_DevolveToken_QuandoCredenciaisCorretas()
    {
        // Arrange
        var mockRepo = new Mock<IUsuarioRepositorio>();

        // Criar usuário com senha hash
        var senha = "123456";
        var usuario = new Usuario("user@teste.com", BCrypt.Net.BCrypt.HashPassword(senha))
        {
            Nome = "Usuário",
            Role = "Usuario"
        };

        mockRepo.Setup(r => r.ObterPorEmailAsync("user@teste.com"))
                .ReturnsAsync(usuario);

        // Mock de IConfiguration com chave JWT forte (32+ caracteres)
        var mockConfig = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
        mockConfig.Setup(c => c["Jwt:Key"])
                  .Returns("esta-chave-tem-mais-de-32-caracteres-para-hs256!"); // 48 chars

        var authServico = new AuthServico(mockRepo.Object, mockConfig.Object);

        // Act
        var resultado = await authServico.LoginAsync("user@teste.com", senha);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Equal(usuario.Nome, resultado.Dados.Nome);
        Assert.Equal(usuario.Role, resultado.Dados.Role);
        Assert.False(string.IsNullOrEmpty(resultado.Dados.Token));
    }

    [Fact]
    public async Task LoginAsync_RetornaErro_QuandoSenhaIncorreta()
    {
        // Arrange
        var mockRepo = new Mock<IUsuarioRepositorio>();
        var usuario = new Usuario("user@teste.com", BCrypt.Net.BCrypt.HashPassword("123456"))
        {
            Nome = "Usuário",
            Role = "Usuario"
        };

        mockRepo.Setup(r => r.ObterPorEmailAsync("user@teste.com"))
                .ReturnsAsync(usuario);

        var mockConfig = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
        mockConfig.Setup(c => c["Jwt:Key"]).Returns("uma-chave-secreta-bem-forte");

        var authServico = new AuthServico(mockRepo.Object, mockConfig.Object);

        // Act
        var resultado = await authServico.LoginAsync("user@teste.com", "senhaerrada");

        // Assert
        Assert.False(resultado.Sucesso);
        Assert.Equal("Senha inválida", resultado.Mensagem);
        Assert.Null(resultado.Dados);
    }

    [Fact]
    public async Task LoginAsync_RetornaErro_QuandoUsuarioNaoExiste()
    {
        // Arrange
        var mockRepo = new Mock<IUsuarioRepositorio>();
        mockRepo.Setup(r => r.ObterPorEmailAsync("inexistente@teste.com"))
                .ReturnsAsync((Usuario?)null);

        var mockConfig = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
        mockConfig.Setup(c => c["Jwt:Key"]).Returns("uma-chave-secreta-bem-forte");

        var authServico = new AuthServico(mockRepo.Object, mockConfig.Object);

        // Act
        var resultado = await authServico.LoginAsync("inexistente@teste.com", "qualquer");

        // Assert
        Assert.False(resultado.Sucesso);
        Assert.Equal("Usuário não encontrado", resultado.Mensagem);
        Assert.Null(resultado.Dados);
    }
}
