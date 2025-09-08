using FinanceiroPessoal.Aplicacao.DTOs;
using FinanceiroPessoal.Apresentacao.Servicos.Interfaces;
using FinanceiroPessoal.Servicos.Api;
using FinanceiroPessoal.ViewModels;
using MudBlazor;
using System.Collections.ObjectModel;

namespace FinanceiroPessoal.Apresentacao.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly IReceitaApiService _receitaService;
        private readonly IDespesaApiService _despesaService;
        private readonly ICategoriaApiService _categoriaService;

        public decimal TotalReceitasMensal { get; private set; }
        public decimal TotalDespesasMensal { get; private set; }
        public decimal SaldoAtual => TotalReceitasMensal - TotalDespesasMensal;

        public ObservableCollection<ReceitaDto> UltimasReceitas { get; private set; } = new();
        public ObservableCollection<DespesaDto> UltimasDespesas { get; private set; } = new();

        public ObservableCollection<string> DespesasLabels { get; private set; } = new();
        public ObservableCollection<double> DespesasData { get; private set; } = new();

        public ObservableCollection<string> ReceitasLabels { get; private set; } = new();
        public ObservableCollection<double> ReceitasData { get; private set; } = new();

        public string[] ReceitasColors { get; } = new string[]
{
    "#4CAF50", "#2196F3", "#00BCD4", "#8BC34A", "#CDDC39", "#FFC107"
};

        public string[] DespesasColors { get; } = new string[]
        {
    "#F44336", "#FF9800", "#FFEB3B", "#E91E63", "#9C27B0", "#673AB7"
        };

        // Totais do mês anterior (para comparação)
        public decimal TotalReceitasMesAnterior { get; set; }
        public decimal TotalDespesasMesAnterior { get; set; }

        // Formatados
        public string ReceitasVariacao => CalcularVariacaoFormatada(TotalReceitasMensal, TotalReceitasMesAnterior);
        public string DespesasVariacao => CalcularVariacaoFormatada(TotalDespesasMensal, TotalDespesasMesAnterior);

        // Indicador positivo/negativo (para cor no card)
        public bool? ReceitaPositiva => CalcularPositivo(TotalReceitasMensal, TotalReceitasMesAnterior);
        public bool? DespesaPositiva => CalcularPositivo(TotalDespesasMensal, TotalDespesasMesAnterior);

        //Enum para filtro de período
        public enum PeriodoFiltro
        {
            Dia,
            Semana,
            Mes,
            Ano,
            Personalizado
        }

        public event Action<string, Severity>? OnSnackbar;

        private void MostrarSnack(string mensagem, Severity severity = Severity.Normal)
        {
            OnSnackbar?.Invoke(mensagem, severity);
        }

        private PeriodoFiltro _periodoSelecionado = PeriodoFiltro.Mes;
        public PeriodoFiltro PeriodoSelecionado
        {
            get => _periodoSelecionado;
            set
            {
                _periodoSelecionado = value;
                _ = CarregarDadosAsync(); // Recarrega dados quando muda o período
            }
        }

        public DateRange PeriodoPersonalizado { get; set; } = new(DateTime.Today.AddDays(-7), DateTime.Today);

        public DateTime? DataInicioPersonalizada { get; set; }
        public DateTime? DataFimPersonalizada { get; set; }

        private string? _categoriaSelecionada;
        public string? CategoriaSelecionada
        {
            get => _categoriaSelecionada;
            set
            {
                _categoriaSelecionada = value;
                _ = CarregarDadosAsync(); // Recarrega tabelas filtradas pela categoria
            }
        }

        public DashboardViewModel(
            IReceitaApiService receitaService,
            IDespesaApiService despesaService,
            ICategoriaApiService categoriaService)
        {
            _receitaService = receitaService;
            _despesaService = despesaService;
            _categoriaService = categoriaService;
        }

        private static string CalcularVariacaoFormatada(decimal atual, decimal anterior)
        {
            if (anterior == 0m)
            {
                if (atual == 0m) return "0%";
                return "—"; // ou "Novo"
            }

            var perc = (atual - anterior) / anterior;
            var sinal = perc >= 0 ? "+" : "";
            return $"{sinal}{perc * 100:0.##}%";
        }

        private static bool? CalcularPositivo(decimal atual, decimal anterior)
        {
            if (anterior == 0m) return null; // não dá para comparar
            return atual >= anterior;
        }

        public async Task CarregarDadosAsync()
        {
            await ExecutarOperacaoAsync(async () =>
            {
                var hoje = DateTime.Now;
                DateTime inicio, fim;
                var inicioMes = new DateTime(hoje.Year, hoje.Month, 1);
                var inicioMesAnterior = inicioMes.AddMonths(-1);
                var fimMesAnterior = inicioMes.AddDays(-1);

                var receitasResp = await _receitaService.ObterTodosAsync();
                var despesasResp = await _despesaService.ObterTodosAsync();
                var categoriasResp = await _categoriaService.ObterTodosAsync();

                if (!receitasResp.Sucesso || !despesasResp.Sucesso || !categoriasResp.Sucesso)
                {
                    MostrarSnack("Erro ao carregar os dados do dashboard", Severity.Error);
                }

                // Define o período com base no filtro selecionado
                switch (PeriodoSelecionado)
                {
                    case PeriodoFiltro.Dia:
                        inicio = hoje.Date;
                        fim = hoje.Date;
                        break;
                    case PeriodoFiltro.Semana:
                        inicio = hoje.AddDays(-(int)hoje.DayOfWeek + 1);
                        fim = hoje;
                        break;
                    case PeriodoFiltro.Mes:
                        inicio = new DateTime(hoje.Year, hoje.Month, 1);
                        fim = hoje;
                        break;
                    case PeriodoFiltro.Ano:
                        inicio = new DateTime(hoje.Year, 1, 1);
                        fim = hoje;
                        break;
                    case PeriodoFiltro.Personalizado:
                        inicio = DataInicioPersonalizada ?? hoje;
                        fim = DataFimPersonalizada ?? hoje;
                        break;
                    default:
                        inicio = hoje;
                        fim = hoje;
                        break;
                }

                if (receitasResp.Sucesso && despesasResp.Sucesso && categoriasResp.Sucesso)
                {
                    var receitas = receitasResp.Dados!.Where(r => r.Data >= inicioMes && r.Data <= hoje).ToList();
                    var despesas = despesasResp.Dados!.Where(d => d.Data >= inicioMes && d.Data <= hoje).ToList();
                    var categorias = categoriasResp.Dados!;

                    // --- Totais do mês atual ---
                    TotalReceitasMensal = receitas.Sum(r => r.Valor);
                    TotalDespesasMensal = despesas.Sum(d => d.Valor);

                    // --- Totais do mês anterior ---
                    var receitasMesAnterior = receitas.Where(r => r.Data >= inicioMesAnterior && r.Data <= fimMesAnterior).ToList();
                    var despesasMesAnterior = despesas.Where(d => d.Data >= inicioMesAnterior && d.Data <= fimMesAnterior).ToList();

                    TotalReceitasMesAnterior = receitasMesAnterior.Sum(r => r.Valor);
                    TotalDespesasMesAnterior = despesasMesAnterior.Sum(d => d.Valor);

                    // --- Últimas receitas e despesas ---
                    UltimasReceitas.Clear();
                    foreach (var r in receitas.OrderByDescending(r => r.Data).Take(5))
                        UltimasReceitas.Add(r);

                    UltimasDespesas.Clear();
                    foreach (var d in despesas.OrderByDescending(d => d.Data).Take(5))
                        UltimasDespesas.Add(d);

                    // --- Agrupamento por categoria (Despesas) ---
                    DespesasLabels.Clear();
                    DespesasData.Clear();
                    var despesasPorCategoria = despesas.GroupBy(d => d.CategoriaId);

                    foreach (var g in despesasPorCategoria)
                    {
                        var categoriaNome = categorias.FirstOrDefault(c => c.Id == g.Key)?.Nome ?? "Outros";
                        DespesasLabels.Add(categoriaNome);
                        DespesasData.Add((double)g.Sum(d => d.Valor));
                    }

                    // --- Agrupamento por categoria (Receitas) ---
                    ReceitasLabels.Clear();
                    ReceitasData.Clear();
                    var receitasPorCategoria = receitas.GroupBy(r => r.CategoriaId);

                    foreach (var g in receitasPorCategoria)
                    {
                        var categoriaNome = categorias.FirstOrDefault(c => c.Id == g.Key)?.Nome ?? "Outros";
                        ReceitasLabels.Add(categoriaNome);
                        ReceitasData.Add((double)g.Sum(r => r.Valor));
                    }
                }
                else
                {
                    MensagemErro = "Erro ao carregar os dados do dashboard.";
                }
            });
        }

        public async Task<List<LancamentoViewModel>> ObterLancamentosAsync(bool receitas)
        {
            DateTime inicio = (PeriodoSelecionado == PeriodoFiltro.Personalizado ? PeriodoPersonalizado.Start : ObterDataInicioPeriodo()) ?? DateTime.Today;
            DateTime fim = (PeriodoSelecionado == PeriodoFiltro.Personalizado ? PeriodoPersonalizado.End : DateTime.Today) ?? DateTime.Today;

            var categoriasResp = await _categoriaService.ObterTodosAsync();
            var categorias = categoriasResp.Dados ?? new List<CategoriaDto>();

            if (receitas)
            {
                var receitasResp = await _receitaService.ObterTodosAsync();
                return receitasResp.Dados!
                    .Where(r => r.Data >= inicio && r.Data <= fim)
                    .Where(r => string.IsNullOrEmpty(CategoriaSelecionada) || r.CategoriaId == CategoriaSelecionada)
                    .Select(r => new LancamentoViewModel
                    {
                        Descricao = r.Descricao,
                        CategoriaNome = categorias.FirstOrDefault(c => c.Id == r.CategoriaId)?.Nome ?? "Outros",
                        Valor = r.Valor,
                        Data = r.Data
                    })
                    .ToList();
            }
            else
            {
                var despesasResp = await _despesaService.ObterTodosAsync();
                return despesasResp.Dados!
                    .Where(d => d.Data >= inicio && d.Data <= fim)
                    .Where(d => string.IsNullOrEmpty(CategoriaSelecionada) || d.CategoriaId == CategoriaSelecionada)
                    .Select(d => new LancamentoViewModel
                    {
                        Descricao = d.Descricao,
                        CategoriaNome = categorias.FirstOrDefault(c => c.Id == d.CategoriaId)?.Nome ?? "Outros",
                        Valor = d.Valor,
                        Data = d.Data
                    })
                    .ToList();
            }
        }


        private DateTime ObterDataInicioPeriodo()
        {
            return PeriodoSelecionado switch
            {
                PeriodoFiltro.Dia => DateTime.Today,
                PeriodoFiltro.Semana => DateTime.Today.AddDays(-7),
                PeriodoFiltro.Mes => new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
                PeriodoFiltro.Ano => new DateTime(DateTime.Today.Year, 1, 1),
                _ => DateTime.Today.AddDays(-7)
            };
        }
    }
}
