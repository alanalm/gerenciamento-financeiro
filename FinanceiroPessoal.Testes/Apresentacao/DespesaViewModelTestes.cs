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

public class DespesaViewModelTestes
{
    [Fact]
    public async Task CarregarDespesasAsync_DevePopularListaDeDespesas()
    {
        // Arrange
        var mockService = new Mock<IDespesaApiService>();
        mockService.Setup(s => s.ObterTodosAsync())
            .ReturnsAsync(RespostaApi<IEnumerable<DespesaDto>>.SucessoResposta(
                new List<DespesaDto>
                {
                    new DespesaDto { Id = "1", Descricao = "Aluguel", Valor = 1000 }
                }
            ));

        var vm = new DespesaViewModel(mockService.Object);

        // Act
        await vm.CarregarDespesasAsync();

        // Assert
        Assert.Single(vm.Despesas);
        Assert.Equal("Aluguel", vm.Despesas.First().Descricao);
        Assert.Null(vm.MensagemErro);
    }

    [Fact]
    public async Task AdicionarDespesaAsync_DeveAdicionarDespesaNaLista()
    {
        // Arrange
        var novaDespesa = new DespesaDto { Id = "2", Descricao = "Internet", Valor = 150 };
        var mockService = new Mock<IDespesaApiService>();
        mockService.Setup(s => s.AdicionarAsync(It.IsAny<CriarDespesaDto>()))
                   .ReturnsAsync(RespostaApi<DespesaDto>.SucessoResposta(novaDespesa));

        var vm = new DespesaViewModel(mockService.Object);

        // Act
        var resultado = await vm.AdicionarDespesaAsync(new CriarDespesaDto { Descricao = "Internet", Valor = 150 });

        // Assert
        Assert.True(resultado.Sucesso);                
        Assert.Single(vm.Despesas);                    
        Assert.Equal("Internet", vm.Despesas.First().Descricao);
        Assert.Equal("Despesa adicionada com sucesso!", vm.MensagemSucesso);
    }


    [Fact]
    public async Task RemoverDespesaAsync_DeveRemoverDespesaDaLista()
    {
        // Arrange
        var despesaExistente = new DespesaDto { Id = "1", Descricao = "Aluguel", Valor = 1000 };
        var mockService = new Mock<IDespesaApiService>();
        mockService.Setup(s => s.RemoverAsync("1"))
            .ReturnsAsync(RespostaApi<bool>.SucessoResposta(true, "Despesa removida com sucesso!"));

        var vm = new DespesaViewModel(mockService.Object);
        vm.Despesas.Add(despesaExistente);

        // Act
        await vm.RemoverDespesaAsync("1");

        // Assert
        Assert.Empty(vm.Despesas);
        Assert.Equal("Despesa removida com sucesso!", vm.MensagemSucesso);
    }

    [Fact]
    public async Task AtualizarDespesaAsync_DeveAtualizarDespesaNaLista()
    {
        // Arrange
        var despesaExistente = new DespesaDto { Id = "1", Descricao = "Aluguel", Valor = 1000 };
        var despesaAtualizada = new DespesaDto { Id = "1", Descricao = "Aluguel Atualizado", Valor = 1200 };

        var mockService = new Mock<IDespesaApiService>();
        mockService.Setup(s => s.AtualizarAsync("1", It.IsAny<AtualizarDespesaDto>()))
            .ReturnsAsync(RespostaApi<DespesaDto>.SucessoResposta(despesaAtualizada));

        var vm = new DespesaViewModel(mockService.Object);
        vm.Despesas.Add(despesaExistente);

        // Act
        await vm.AtualizarDespesaAsync("1", new AtualizarDespesaDto { Descricao = "Aluguel Atualizado", Valor = 1200 });

        // Assert
        Assert.Single(vm.Despesas);
        Assert.Equal("Aluguel Atualizado", vm.Despesas.First().Descricao);
        Assert.Equal(1200, vm.Despesas.First().Valor);
        Assert.Equal("Despesa atualizada com sucesso!", vm.MensagemSucesso);
    }
}
