using Xunit;
using Moq;
using FinanceiroPessoal.ViewModels;
using FinanceiroPessoal.Servicos.Api;
using FinanceiroPessoal.Aplicacao.DTOs;
using FinanceiroPessoal.Dominio.Comum;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class CategoriaViewModelTestes
{
    [Fact]
    public async Task CarregarCategoriasAsync_DevePopularListaDeCategorias()
    {
        // Arrange (Preparação)
        var mockService = new Mock<ICategoriaApiService>();
        mockService.Setup(s => s.ObterTodosAsync())
            .ReturnsAsync(RespostaApi<IEnumerable<CategoriaDto>>.SucessoResposta(
                new List<CategoriaDto> { new CategoriaDto { Id = "1", Nome = "Transporte" } }
            ));

        var vm = new CategoriaViewModel(mockService.Object);

        // Act (Ação)
        await vm.CarregarCategoriasAsync();

        // Assert (Verificação)
        Assert.Single(vm.Categorias);
        Assert.Equal("Transporte", vm.Categorias.First().Nome);
        Assert.Null(vm.MensagemErro);
    }

    [Fact]
    public async Task AdicionarCategoriaAsync_DeveAdicionarCategoriaNaLista()
    {
        // Arrange
        var novaCategoria = new CategoriaDto { Id = "2", Nome = "Lazer" };
        var mockService = new Mock<ICategoriaApiService>();
        mockService.Setup(s => s.AdicionarAsync(It.IsAny<CriarCategoriaDto>()))
            .ReturnsAsync(RespostaApi<CategoriaDto>.SucessoResposta(novaCategoria));

        var vm = new CategoriaViewModel(mockService.Object);

        // Act
        await vm.AdicionarCategoriaAsync(new CriarCategoriaDto { Nome = "Lazer" });

        // Assert
        Assert.Single(vm.Categorias);
        Assert.Equal("Lazer", vm.Categorias.First().Nome);
        Assert.Equal("Categoria adicionada com sucesso!", vm.MensagemSucesso);
    }
}
