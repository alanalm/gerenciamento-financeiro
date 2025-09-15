using FinanceiroPessoal.Aplicacao.DTOs;
using FinanceiroPessoal.Aplicacao.Interfaces;
using FinanceiroPessoal.Dominio.Comum;
using FinanceiroPessoal.Dominio.Enums;
using FinanceiroPessoal.Servicos.Api;
using FinanceiroPessoal.ViewModels;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

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
        var mockApiService = new Mock<ICategoriaApiService>();
        var viewModel = new CategoriaViewModel(mockApiService.Object);

        var dto = new CriarCategoriaDto { Nome = "Alimentação", Tipo = TipoCategoria.Despesa };

        // Simula que o serviço de API retorna sucesso
        mockApiService
            .Setup(s => s.AdicionarAsync(It.IsAny<CriarCategoriaDto>()))
            .ReturnsAsync(RespostaApi<CategoriaDto>.SucessoResposta(new CategoriaDto
            {
                Id = "1",
                Nome = dto.Nome,
                Tipo = dto.Tipo
            }));

        // Act
        await viewModel.AdicionarCategoriaAsync(dto);

        // Assert
        Assert.Single(viewModel.Categorias);
        Assert.Equal("Alimentação", viewModel.Categorias[0].Nome);
    }
}
