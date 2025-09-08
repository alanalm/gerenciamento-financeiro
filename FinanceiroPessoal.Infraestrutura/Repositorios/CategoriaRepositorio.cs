using FinanceiroPessoal.Dominio.Entidades;
using FinanceiroPessoal.Dominio.Enums;
using FinanceiroPessoal.Dominio.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;

namespace FinanceiroPessoal.Infraestrutura.Repositorios
{
    public class CategoriaRepositorio : CosmosDbRepositorioBase<Categoria>, IRepositorioCategorias
    {
        public CategoriaRepositorio(CosmosClient cosmosClient, string databaseName, string containerName)
            : base(cosmosClient, databaseName, containerName)
        {
        }

        public async Task<IEnumerable<Categoria>> ObterPorUsuarioAsync(Guid usuarioId)
        {
            var query = _container.GetItemLinqQueryable<Categoria>(true)
                                  .Where(c => c.UsuarioId == usuarioId)
                                  .ToFeedIterator();

            var resultados = new List<Categoria>();
            while (query.HasMoreResults)
            {
                resultados.AddRange(await query.ReadNextAsync());
            }
            return resultados;
        }

        public async Task<IEnumerable<Categoria>> ObterPorTipoAsync(Guid usuarioId, TipoCategoria tipo)
        {
            var query = _container.GetItemLinqQueryable<Categoria>(true)
                                  .Where(c => c.UsuarioId == usuarioId && c.Tipo == tipo)
                                  .ToFeedIterator();

            var resultados = new List<Categoria>();
            while (query.HasMoreResults)
            {
                resultados.AddRange(await query.ReadNextAsync());
            }
            return resultados;
        }

        public async Task<Categoria?> ObterPorIdAsync(string id, Guid usuarioId)
        {
            var categoria = await base.ObterPorIdAsync(id);
            if (categoria == null || categoria.UsuarioId != usuarioId)
                return null;
            return categoria;
        }

        public async Task RemoverAsync(string id, Guid usuarioId)
        {
            var categoria = await ObterPorIdAsync(id, usuarioId);
            if (categoria != null)
            {
                await base.RemoverAsync(id);
            }
        }
    }
}
