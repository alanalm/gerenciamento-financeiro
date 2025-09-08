using FinanceiroPessoal.ViewModels;
using Microsoft.AspNetCore.Components;

namespace FinanceiroPessoal.Apresentacao.Shared
{
    public abstract class BaseModal<TViewModel> : ComponentBase
       where TViewModel : BaseViewModel
    {
        [Parameter] public TViewModel ViewModel { get; set; }

        protected override void OnInitialized()
        {
            // limpa estado antigo sempre que o modal abre
            ViewModel?.LimparTodosErros();
            ViewModel?.LimparMensagens();
            base.OnInitialized();
        }
    }
}
