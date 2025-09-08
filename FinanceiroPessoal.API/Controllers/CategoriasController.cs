using FinanceiroPessoal.Aplicacao.DTOs;
using FinanceiroPessoal.Aplicacao.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinanceiroPessoal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaServico _categoriaServico;
        private readonly Guid usuarioTeste = Guid.Parse("11111111-1111-1111-1111-111111111111");

        public CategoriasController(ICategoriaServico categoriaServico)
        {
            _categoriaServico = categoriaServico;
        }

        private Guid GetUsuarioId() => usuarioTeste;

        [HttpGet]
        public async Task<IActionResult> ObterTodos()
        {
            var resultado = await _categoriaServico.ObterTodasCategorias(GetUsuarioId());
            return resultado.Sucesso ? Ok(resultado) : BadRequest(resultado);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(string id)
        {
            var resultado = await _categoriaServico.ObterCategoriaPorId(id, GetUsuarioId());
            return resultado.Sucesso ? Ok(resultado) : NotFound(resultado);
        }

        [HttpPost]
        public async Task<IActionResult> Adicionar([FromBody] CriarCategoriaDto dto)
        {
            var resultado = await _categoriaServico.AdicionarCategoria(dto, GetUsuarioId());

            return resultado.Sucesso 
                ? CreatedAtAction(nameof(ObterPorId), new { id = resultado.Dados!.Id }, resultado) 
                : BadRequest(resultado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(string id, [FromBody] AtualizarCategoriaDto dto)
        {
            var resultado = await _categoriaServico.AtualizarCategoria(id, dto, GetUsuarioId());
            return resultado.Sucesso ? Ok(resultado) : NotFound(resultado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remover(string id)
        {
            var resultado = await _categoriaServico.RemoverCategoria(id, GetUsuarioId());
            return resultado.Sucesso ? Ok(resultado) : NotFound(resultado);
        }
    }
}
