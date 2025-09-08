namespace FinanceiroPessoal.Apresentacao.ViewModels
{
    public class LancamentoViewModel
    {
        public string Descricao { get; set; } = string.Empty;
        public string CategoriaNome { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public DateTime? Data { get; set; }
    }
}
