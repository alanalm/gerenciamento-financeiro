using FinanceiroPessoal.Dominio.Entidades;
using FinanceiroPessoal.Dominio.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace FinanceiroPessoal.Infraestrutura.Repositorios
{
    public class UsuarioRepositorio : CosmosDbRepositorioBase<Usuario>, IUsuarioRepositorio
    {
        public UsuarioRepositorio(CosmosClient client, string databaseName, string containerName)
            : base(client, databaseName, containerName)
        {
        }

        public async Task<Usuario?> ObterPorEmailAsync(string email)
        {
            var iterator = _container.GetItemLinqQueryable<Usuario>(true)
                                     .Where(u => u.Email == email)
                                     .ToFeedIterator();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                var usuario = response.FirstOrDefault();
                if (usuario != null)
                    return usuario;
            }

            return null;
        }

        public async Task<Usuario?> ObterPorIdAsync(string id)
        {
            try
            {
                var response = await _container.ReadItemAsync<Usuario>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException)
            {
                return null;
            }
        }

        public async Task CriarAsync(Usuario usuario)
        {
            await _container.CreateItemAsync(usuario, new PartitionKey(usuario.Id));
        }

        public async Task<IEnumerable<Usuario>> ObterTodosAsync()
        {
            var iterator = _container.GetItemLinqQueryable<Usuario>(true)
                                     .ToFeedIterator();

            var results = new List<Usuario>();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }
    }
}
