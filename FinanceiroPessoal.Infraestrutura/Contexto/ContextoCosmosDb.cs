using FinanceiroPessoal.Infraestrutura.Configuracoes;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace FinanceiroPessoal.Infraestrutura.Contexto
{
    public class ContextoCosmosDb
    {
        private readonly CosmosClient _cosmosClient;
        private readonly CosmosDbConfiguracoes _configuracoes;

        public ContextoCosmosDb(IOptions<CosmosDbConfiguracoes> configuracoes)
        {
            _configuracoes = configuracoes.Value;
            _cosmosClient = new CosmosClient(_configuracoes.Endpoint, _configuracoes.Chave);

            InicializarAsync().GetAwaiter().GetResult();
        }

        public Container ObterContainerDespesas() =>
            _cosmosClient.GetContainer(_configuracoes.NomeDatabase, _configuracoes.ContainerDespesas);

        public Container ObterContainerReceitas() =>
            _cosmosClient.GetContainer(_configuracoes.NomeDatabase, _configuracoes.ContainerReceitas);

        public Container ObterContainerCategorias() =>
            _cosmosClient.GetContainer(_configuracoes.NomeDatabase, _configuracoes.ContainerCategorias);

        private async Task InicializarAsync()
        {
            var database = await _cosmosClient.CreateDatabaseIfNotExistsAsync(_configuracoes.NomeDatabase);
            await database.Database.CreateContainerIfNotExistsAsync(_configuracoes.ContainerDespesas, "/id");
            await database.Database.CreateContainerIfNotExistsAsync(_configuracoes.ContainerReceitas, "/id");
            await database.Database.CreateContainerIfNotExistsAsync(_configuracoes.ContainerCategorias, "/id");
        }
    }
}
