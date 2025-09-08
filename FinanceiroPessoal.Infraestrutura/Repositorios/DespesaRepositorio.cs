using FinanceiroPessoal.Dominio.Entidades;
using FinanceiroPessoal.Dominio.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace FinanceiroPessoal.Infraestrutura.Repositorios
{
    public class DespesaRepositorio : CosmosDbRepositorioBase<Despesa>, IRepositorioDespesas
    {
        public DespesaRepositorio(CosmosClient cosmosClient, string databaseName, string containerName)
            : base(cosmosClient, databaseName, containerName)
        {
        }

        public async Task<IEnumerable<Despesa>> ObterPorUsuarioAsync(Guid usuarioId)
        {
            var query = _container.GetItemLinqQueryable<Despesa>(true)
                                  .Where(d => d.UsuarioId == usuarioId)
                                  .ToFeedIterator();

            var resultados = new List<Despesa>();
            while (query.HasMoreResults)
            {
                resultados.AddRange(await query.ReadNextAsync());
            }
            return resultados;
        }

        public async Task<Despesa?> ObterPorIdAsync(string id, Guid usuarioId)
        {
            var despesa = await base.ObterPorIdAsync(id);
            if (despesa == null || despesa.UsuarioId != usuarioId)
                return null;
            return despesa;
        }

        public async Task RemoverAsync(string id, Guid usuarioId)
        {
            var despesa = await ObterPorIdAsync(id, usuarioId);
            if (despesa != null)
            {
                await base.RemoverAsync(id);
            }
        }
    }
}
