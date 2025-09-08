using FinanceiroPessoal.Aplicacao.DTOs;
using FinanceiroPessoal.Apresentacao.Servicos.Interfaces;
using FinanceiroPessoal.Apresentacao.ViewModels;
using FinanceiroPessoal.Dominio.Comum;
using FinanceiroPessoal.Servicos.Api;
using FinanceiroPessoal.ViewModels;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class ReceitaViewModelTestes
{
    [Fact]
    public async Task CarregarReceitasAsync_DevePopularListaDeReceitas()
    {
        // Arrange
        var mockService = new Mock<IReceitaApiService>();
        mockService.Setup(s => s.ObterTodosAsync())
            .ReturnsAsync(RespostaApi<IEnumerable<ReceitaDto>>.SucessoResposta(
                new List<ReceitaDto>
                {
                    new ReceitaDto { Id = "1", Descricao = "Salário", Valor = 5000 }
                }
            ));

        var vm = new ReceitaViewModel(mockService.Object);

        // Act
        await vm.CarregarReceitasAsync();

        // Assert
        Assert.Single(vm.Receitas);
        Assert.Equal("Salário", vm.Receitas.First().Descricao);
        Assert.Null(vm.MensagemErro);
    }

    [Fact]
    public async Task AdicionarReceitaAsync_DeveAdicionarReceitaNaLista()
    {
        // Arrange
        var novaReceita = new ReceitaDto { Id = "2", Descricao = "Freelance", Valor = 800 };
        var mockService = new Mock<IReceitaApiService>();
        mockService.Setup(s => s.AdicionarAsync(It.IsAny<CriarReceitaDto>()))
            .ReturnsAsync(RespostaApi<ReceitaDto>.SucessoResposta(novaReceita));

        var vm = new ReceitaViewModel(mockService.Object);

        // Act
        await vm.AdicionarReceitaAsync(new CriarReceitaDto { Descricao = "Freelance", Valor = 800 });

        // Assert
        Assert.Single(vm.Receitas);
        Assert.Equal("Freelance", vm.Receitas.First().Descricao);
        Assert.Equal("Receita adicionada com sucesso!", vm.MensagemSucesso);
    }

    [Fact]
    public async Task RemoverReceitaAsync_DeveRemoverReceitaDaLista()
    {
        // Arrange
        var receitaExistente = new ReceitaDto { Id = "1", Descricao = "Salário", Valor = 5000 };
        var mockService = new Mock<IReceitaApiService>();
        mockService.Setup(s => s.RemoverAsync("1"))
            .ReturnsAsync(RespostaApi<bool>.SucessoResposta(true));

        var vm = new ReceitaViewModel(mockService.Object);
        vm.Receitas.Add(receitaExistente);

        // Act
        await vm.RemoverReceitaAsync("1");

        // Assert
        Assert.Empty(vm.Receitas);
        Assert.Equal("Receita removida com sucesso!", vm.MensagemSucesso);
    }

    [Fact]
    public async Task AtualizarReceitaAsync_DeveAtualizarReceitaNaLista()
    {
        // Arrange
        var receitaExistente = new ReceitaDto { Id = "1", Descricao = "Salário", Valor = 5000 };
        var receitaAtualizada = new ReceitaDto { Id = "1", Descricao = "Salário Atualizado", Valor = 5500 };

        var mockService = new Mock<IReceitaApiService>();
        mockService.Setup(s => s.AtualizarAsync("1", It.IsAny<AtualizarReceitaDto>()))
            .ReturnsAsync(RespostaApi<ReceitaDto>.SucessoResposta(receitaAtualizada));

        var vm = new ReceitaViewModel(mockService.Object);
        vm.Receitas.Add(receitaExistente);

        // Act
        await vm.AtualizarReceitaAsync("1", new AtualizarReceitaDto { Descricao = "Salário Atualizado", Valor = 5500 });

        // Assert
        Assert.Single(vm.Receitas);
        Assert.Equal("Salário Atualizado", vm.Receitas.First().Descricao);
        Assert.Equal(5500, vm.Receitas.First().Valor);
        Assert.Equal("Receita atualizada com sucesso!", vm.MensagemSucesso);
    }
}
