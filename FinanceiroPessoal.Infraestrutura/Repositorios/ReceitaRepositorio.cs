using FinanceiroPessoal.Dominio.Entidades;
using FinanceiroPessoal.Dominio.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System.Collections.Concurrent;

namespace FinanceiroPessoal.Infraestrutura.Repositorios
{
    public class ReceitaRepositorio : CosmosDbRepositorioBase<Receita>, IRepositorioReceitas
    {
        public ReceitaRepositorio(CosmosClient client, string db, string container)
        : base(client, db, container) { }

        public async Task<Receita?> ObterPorIdAsync(string id, Guid usuarioId)
        {
            try
            {
                var response = await _container.ReadItemAsync<Receita>(id, new PartitionKey(usuarioId.ToString()));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<IEnumerable<Receita>> ObterPorUsuarioAsync(Guid usuarioId)
        {
            var query = _container.GetItemLinqQueryable<Receita>()
                .Where(r => r.UsuarioId == usuarioId)
                .ToFeedIterator();

            var results = new List<Receita>();
            while (query.HasMoreResults)
                results.AddRange(await query.ReadNextAsync());

            return results;
        }

        public async Task<IEnumerable<Receita>> ObterPorPeriodoAsync(Guid usuarioId, DateTime inicio, DateTime fim)
        {
            var query = _container.GetItemLinqQueryable<Receita>()
                .Where(r => r.UsuarioId == usuarioId && r.DataRecebimento >= inicio && r.DataRecebimento <= fim)
                .ToFeedIterator();

            var results = new List<Receita>();
            while (query.HasMoreResults)
                results.AddRange(await query.ReadNextAsync());

            return results;
        }

        public async Task RemoverAsync(string id, Guid usuarioId)
        {
            await _container.DeleteItemAsync<Receita>(id, new PartitionKey(usuarioId.ToString()));
        }
    }
}
