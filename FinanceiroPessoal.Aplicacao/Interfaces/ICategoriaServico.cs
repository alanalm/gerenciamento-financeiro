using FinanceiroPessoal.Aplicacao.DTOs;
using FinanceiroPessoal.Dominio.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceiroPessoal.Aplicacao.Interfaces
{
    public interface ICategoriaServico
    {
        Task<RespostaApi<CategoriaDto>> AdicionarCategoria(CriarCategoriaDto categoriaDto, Guid usuarioId);
        Task<RespostaApi<CategoriaDto>> ObterCategoriaPorId(string id, Guid usuarioId);
        Task<RespostaApi<IEnumerable<CategoriaDto>>> ObterTodasCategorias(Guid usuarioId);
        Task<RespostaApi<CategoriaDto>> AtualizarCategoria(string id, AtualizarCategoriaDto categoriaDto, Guid usuarioId);
        Task<RespostaApi<bool>> RemoverCategoria(string id, Guid usuarioId);
    }
}
