using FinanceiroPessoal.Dominio.Comum;
using FluentValidation;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FinanceiroPessoal.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        /// Interface para validação de dados em ViewModels.
        public IEnumerable<string> MensagensErro => _erros.SelectMany(e => e.Value);
        /// Evento disparado quando uma propriedade é alterada.
        private readonly Dictionary<string, List<string>> _erros = new();

        // Evento para notificar alterações de erros (opcional, útil para UI)
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        // Indica se há erros de validação
        public bool TemErros => _erros.Count > 0;


        private bool _estaCarregando;
        private string? _mensagemErro;
        private string? _mensagemSucesso;

        public bool EstaCarregando
        {
            get => _estaCarregando;
            set
            {
                _estaCarregando = value;
                OnPropertyChanged();
            }
        }

        public string? MensagemErro
        {
            get => _mensagemErro;
            set
            {
                _mensagemErro = value;
                OnPropertyChanged();
            }
        }

        public string? MensagemSucesso
        {
            get => _mensagemSucesso;
            protected set
            {
                _mensagemSucesso = value;
                OnPropertyChanged();
            }
        }

        public event Action? ErrosAtualizados;

        protected void NotificarErros()
        {
            ErrosAtualizados?.Invoke();
        }

        /// <summary>
        /// Executa uma operação assíncrona com tratamento de erros e estado de carregamento.
        /// </summary>
        protected async Task ExecutarOperacaoAsync(Func<Task> acao)
        {
            try
            {
                EstaCarregando = true;
                MensagemErro = null;
                MensagemSucesso = null;

                await acao();
            }
            catch (Exception ex)
            {
                MensagemErro = ex.Message; // depois trocar para algo mais amigável
            }
            finally
            {
                EstaCarregando = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propriedade = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propriedade));
        }

     public IEnumerable<string>? GetErros(string propriedade)
        {
            if (string.IsNullOrEmpty(propriedade))
                return null;

            return _erros.ContainsKey(propriedade) ? _erros[propriedade] : null;
        }

        // Método que deve ser chamado por subclasses para definir erros
        protected void DefinirErroValidacao(string propriedade, string erro)
        {
            if (!_erros.ContainsKey(propriedade))
                _erros[propriedade] = new List<string>();

            if (!_erros[propriedade].Contains(erro))
            {
                _erros[propriedade].Add(erro);
                OnErrorsChanged(propriedade);
                OnPropertyChanged(nameof(TemErros));
            }
        }

        // Limpa os erros de uma propriedade
        public void LimparErros(string propriedade)
        {
            if (_erros.Remove(propriedade))
            {
                OnErrorsChanged(propriedade);
                OnPropertyChanged(nameof(TemErros));
            }
        }

        // Limpa todos os erros
        public void LimparTodosErros()
        {
            var propriedades = _erros.Keys.ToList();
            _erros.Clear();
            foreach (var prop in propriedades)
            {
                OnErrorsChanged(prop);
            }
            OnPropertyChanged(nameof(TemErros));
        }

        public void LimparMensagens()
        {
            MensagemErro = null;
            MensagemSucesso = null;
        }

        protected virtual void OnErrorsChanged(string propriedade)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propriedade));
        }

        public bool ValidarDto<T>(AbstractValidator<T> validador, T dto)
        {
            LimparTodosErros();

            var resultado = validador.Validate(dto);

            if (!resultado.IsValid)
            {
                foreach (var erro in resultado.Errors)
                {
                    DefinirErroValidacao(erro.PropertyName, erro.ErrorMessage);
                }

                return false;
            }

            return true;
        }
        public IEnumerable<string> GetPropriedadesComErros()
        {
            return _erros.Keys.ToList();
        }
        public RespostaApi<T> ValidarDtoResultado<T>(AbstractValidator<T> validador, T dto)
        {
            LimparTodosErros();

            var resultadoValidacao = validador.Validate(dto);

            if (!resultadoValidacao.IsValid)
            {
                foreach (var erro in resultadoValidacao.Errors)
                {
                    DefinirErroValidacao(erro.PropertyName, erro.ErrorMessage);
                }

                var mensagens = resultadoValidacao.Errors
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return RespostaApi<T>.ErroResposta(string.Join("; ", mensagens));
            }

            return RespostaApi<T>.SucessoResposta(dto, "Validação concluída.");
        }

        public async Task<RespostaApi<T>> ValidarDtoResultadoAsync<T>(IValidator<T> validador, T dto)
        {
            LimparTodosErros();

            var resultadoValidacao = await validador.ValidateAsync(dto);

            if (!resultadoValidacao.IsValid)
            {
                foreach (var erro in resultadoValidacao.Errors)
                {
                    DefinirErroValidacao(erro.PropertyName, erro.ErrorMessage);
                }

                var mensagens = resultadoValidacao.Errors
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return RespostaApi<T>.ErroResposta(string.Join("; ", mensagens));
            }

            return RespostaApi<T>.SucessoResposta(dto, "Validação concluída.");
        }
    }
}
