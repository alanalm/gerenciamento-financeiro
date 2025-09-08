using FinanceiroPessoal.Aplicacao.DTOs;
using FinanceiroPessoal.Aplicacao.Validadores;
using FinanceiroPessoal.Apresentacao.Servicos.Interfaces;
using FinanceiroPessoal.Dominio.Comum;
using FinanceiroPessoal.ViewModels;
using System.Collections.ObjectModel;

namespace FinanceiroPessoal.Apresentacao.ViewModels
{
    public class ReceitaViewModel : BaseViewModel
    {
        private readonly IReceitaApiService _receitaService;
        private readonly ValidadorCriarReceitas _validadorCriar = new();
        private readonly ValidadorAtualizarReceitas _validadorAtualizar = new();

        public ObservableCollection<ReceitaDto> Receitas { get; private set; } = new();

        public ReceitaViewModel(IReceitaApiService receitaService)
        {
            _receitaService = receitaService;
        }

        public async Task CarregarReceitasAsync()
        {
            await ExecutarOperacaoAsync(async () =>
            {
                var resposta = await _receitaService.ObterTodosAsync();
                if (resposta.Sucesso && resposta.Dados != null)
                {
                    Receitas = new ObservableCollection<ReceitaDto>(resposta.Dados);
                }
                else
                {
                    MensagemErro = resposta.Mensagem ?? "Erro ao carregar receitas.";
                }
            });
        }

        public async Task<RespostaApi<ReceitaDto>> AdicionarReceitaAsync(CriarReceitaDto novaReceita)
        {
            var validacao = ValidarDtoResultado(_validadorCriar, novaReceita);

            if (!validacao.Sucesso)
                return RespostaApi<ReceitaDto>.ErroResposta(validacao.Mensagem);

            RespostaApi<ReceitaDto>? resultado = null;

            await ExecutarOperacaoAsync(async () =>
            {
                var resposta = await _receitaService.AdicionarAsync(novaReceita);
                if (resposta.Sucesso)
                {
                    await CarregarReceitasAsync(); 
                    MensagemSucesso = "Receita adicionada com sucesso!";
                }
                else
                {
                    MensagemErro = resposta.Mensagem ?? "Erro ao adicionar receita.";
                }
            });
            return resultado ?? RespostaApi<ReceitaDto>.ErroResposta("Erro inesperado ao adicionar receita.");
        }

        public async Task<RespostaApi<ReceitaDto>> AtualizarReceitaAsync(string id, AtualizarReceitaDto receitaAtualizada)
        {
            var validacao = ValidarDtoResultado(_validadorAtualizar, receitaAtualizada);

            if (!validacao.Sucesso)
                return RespostaApi<ReceitaDto>.ErroResposta(validacao.Mensagem);

            RespostaApi<ReceitaDto>? resultado = null;

            await ExecutarOperacaoAsync(async () =>
            {
                var resposta = await _receitaService.AtualizarAsync(id, receitaAtualizada);
                if (resposta.Sucesso)
                {
                    await CarregarReceitasAsync();
                    MensagemSucesso = "Receita atualizada com sucesso!";
                }
                else
                {
                    MensagemErro = resposta.Mensagem ?? "Erro ao atualizar receita.";
                }
            });
            return resultado ?? RespostaApi<ReceitaDto>.ErroResposta("Erro inesperado ao adicionar receita.");
        }

        public async Task RemoverReceitaAsync(string id)
        {
            await ExecutarOperacaoAsync(async () =>
            {
                var resposta = await _receitaService.RemoverAsync(id);
                if (resposta.Sucesso)
                {
                    await CarregarReceitasAsync(); // 🔄 garante refresh
                    MensagemSucesso = "Receita removida com sucesso!";
                }
                else
                {
                    MensagemErro = resposta.Mensagem ?? "Erro ao remover receita.";
                }
            });
        }
    }
}
