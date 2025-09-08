using FinanceiroPessoal.Aplicacao.DTOs;
using FinanceiroPessoal.Aplicacao.Validadores;
using FinanceiroPessoal.Apresentacao.Servicos.Interfaces;
using FinanceiroPessoal.Dominio.Comum;
using FinanceiroPessoal.ViewModels;
using System.Collections.ObjectModel;

namespace FinanceiroPessoal.Apresentacao.ViewModels
{
    public class DespesaViewModel : BaseViewModel
    {
        private readonly IDespesaApiService _despesaService;
        private readonly ValidadorCriarDespesas _validadorCriar = new();
        private readonly ValidadorAtualizarDespesas _validadorAtualizar = new();

        public ObservableCollection<DespesaDto> Despesas { get; private set; } = new();

        public DespesaViewModel(IDespesaApiService despesaService)
        {
            _despesaService = despesaService;
        }

        public async Task CarregarDespesasAsync()
        {
            await ExecutarOperacaoAsync(async () =>
            {
                var resultado = await _despesaService.ObterTodosAsync();
                if (resultado.Sucesso && resultado.Dados != null)
                {
                    Despesas.Clear();
                    foreach (var d in resultado.Dados)
                        Despesas.Add(d);
                }
                else
                {
                    MensagemErro = resultado.Mensagem ?? "Erro ao carregar despesas.";
                }
            });
        }

        public async Task<RespostaApi<DespesaDto>> AdicionarDespesaAsync(CriarDespesaDto novaDespesa)
        {
            var validacao = ValidarDtoResultado(_validadorCriar, novaDespesa);

            if (!validacao.Sucesso)
                return RespostaApi<DespesaDto>.ErroResposta(validacao.Mensagem);

            RespostaApi<DespesaDto>? resultado = null;

            await ExecutarOperacaoAsync(async () =>
            {
                var resposta = await _despesaService.AdicionarAsync(novaDespesa);
                if (resposta.Sucesso)
                {
                    await CarregarDespesasAsync();
                    MensagemSucesso = "Despesa adicionada com sucesso!";
                }
                else
                {
                    MensagemErro = resposta.Mensagem ?? "Erro ao adicionar despesa.";
                }
            });
            return resultado ?? RespostaApi<DespesaDto>.ErroResposta("Erro inesperado ao adicionar despesa.");
        }


        public async Task<RespostaApi<DespesaDto>> AtualizarDespesaAsync(string id, AtualizarDespesaDto despesaAtualizada)
        {
            var validacao = ValidarDtoResultado(_validadorAtualizar, despesaAtualizada);

            if (!validacao.Sucesso)
                return RespostaApi<DespesaDto>.ErroResposta(validacao.Mensagem);

            RespostaApi<DespesaDto>? resultado = null;

            await ExecutarOperacaoAsync(async () =>
            {
                var resposta = await _despesaService.AtualizarAsync(id, despesaAtualizada);
                if (resposta.Sucesso)
                {
                    await CarregarDespesasAsync();
                    MensagemSucesso = "Despesa atualizada com sucesso!";
                }
                else
                {
                    MensagemErro = resposta.Mensagem ?? "Erro ao atualizar despesa.";
                }
            });
            return resultado ?? RespostaApi<DespesaDto>.ErroResposta("Erro inesperado ao atualizar despesa.");
        }

        public async Task RemoverDespesaAsync(string id)
        {
            await ExecutarOperacaoAsync(async () =>
            {
                var resultado = await _despesaService.RemoverAsync(id);
                if (resultado.Sucesso)
                {
                    var despesa = Despesas.FirstOrDefault(d => d.Id == id);
                    if (despesa != null) Despesas.Remove(despesa);
                    MensagemSucesso = resultado.Mensagem ?? "Despesa removida com sucesso!";
                }
                else
                {
                    MensagemErro = resultado.Mensagem ?? "Erro ao remover despesa.";
                }
            });
        }
    }
}
