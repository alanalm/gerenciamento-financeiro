using Newtonsoft.Json;

namespace FinanceiroPessoal.Dominio.Comum
{
    public abstract class EntidadeBase
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public Guid UsuarioId { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
    }

}
