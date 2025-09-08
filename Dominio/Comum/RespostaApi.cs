namespace FinanceiroPessoal.Dominio.Comum
{
    public class RespostaApi<T>
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public T Dados { get; set; }
        public List<string> Erros { get; set; }

        public RespostaApi()
        {
            Erros = new List<string>();
        }

        public static RespostaApi<T> SucessoResposta(T dados, string mensagem = "")
        {
            return new RespostaApi<T> { Sucesso = true, Dados = dados, Mensagem = mensagem };
        }

        public static RespostaApi<T> ErroResposta(string mensagem, List<string> erros = null)
        {
            return new RespostaApi<T> { Sucesso = false, Mensagem = mensagem, Erros = erros ?? new List<string>() };
        }
    }
}
