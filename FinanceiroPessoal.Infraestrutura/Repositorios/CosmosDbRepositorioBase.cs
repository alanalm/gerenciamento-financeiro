using FinanceiroPessoal.Dominio.Comum;
using FinanceiroPessoal.Dominio.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System.Linq.Expressions;
using Container = Microsoft.Azure.Cosmos.Container;

namespace FinanceiroPessoal.Infraestrutura.Repositorios
{
    public class CosmosDbRepositorioBase<T> : IRepositorioBase<T> where T : EntidadeBase
    {
        protected readonly Container _container;

        public CosmosDbRepositorioBase(CosmosClient cosmosClient, string databaseName, string containerName)
        {
            _container = cosmosClient.GetContainer(databaseName, containerName);
        }

        public async Task<T> AdicionarAsync(T entidade)
        {
            entidade.Id = Guid.NewGuid().ToString();
            entidade.DataCriacao = DateTime.UtcNow;
            var response = await _container.CreateItemAsync(entidade, new PartitionKey(entidade.Id));
            return response.Resource;
        }

        public async Task<T> ObterPorIdAsync(string id)
        {
            try
            {
                ItemResponse<T> response = await _container.ReadItemAsync<T>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<IEnumerable<T>> ObterTodosAsync()
        {
            var query = _container.GetItemQueryIterator<T>(new QueryDefinition("SELECT * FROM c"));
            var results = new List<T>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ReadNextAsync());
            }
            return results;
        }

        public async Task<IEnumerable<T>> BuscarAsync(Expression<Func<T, bool>> predicate)
        {
            var query = _container.GetItemLinqQueryable<T>(true).Where(predicate).ToFeedIterator();
            var results = new List<T>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ReadNextAsync());
            }
            return results;
        }

        public async Task AtualizarAsync(T entidade)
        {
            entidade.DataAtualizacao = DateTime.UtcNow;
            await _container.ReplaceItemAsync(entidade, entidade.Id, new PartitionKey(entidade.Id));
        }

        public async Task RemoverAsync(string id)
        {
            await _container.DeleteItemAsync<T>(id, new PartitionKey(id));
        }
    }
}
