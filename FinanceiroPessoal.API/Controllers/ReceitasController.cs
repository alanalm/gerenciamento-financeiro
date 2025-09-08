using FinanceiroPessoal.Aplicacao.DTOs;
using FinanceiroPessoal.Aplicacao.Interfaces;
using FinanceiroPessoal.Aplicacao.Servicos;
using FinanceiroPessoal.Dominio.Comum;
using Microsoft.AspNetCore.Mvc;

namespace FinanceiroPessoal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReceitasController : ControllerBase
    {
        private readonly IReceitaServico _receitaServico;
        private readonly Guid usuarioTeste = Guid.Parse("11111111-1111-1111-1111-111111111111");

        public ReceitasController(IReceitaServico receitaServico)
        {
            _receitaServico = receitaServico;
        }

        // Simulação de UsuarioId, em um cenário real vai vir do JWT
        private Guid GetUsuarioId() => usuarioTeste;

        [HttpPost]
        public async Task<ActionResult<RespostaApi<ReceitaDto>>> Post([FromBody] CriarReceitaDto receitaDto)
        {
            var result = await _receitaServico.AdicionarReceita(receitaDto, GetUsuarioId());
            if (result.Sucesso)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RespostaApi<ReceitaDto>>> Get(string id)
        {
            var result = await _receitaServico.ObterReceitaPorId(id, GetUsuarioId());
            if (result.Sucesso)
            {
                return Ok(result);
            }
            return NotFound(result);
        }

        [HttpGet]
        public async Task<ActionResult<RespostaApi<IEnumerable<ReceitaDto>>>> GetPorUsuario()
        {
            var result = await _receitaServico.ObterReceitasPorUsuario(GetUsuarioId());
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<RespostaApi<ReceitaDto>>> Put(string id, [FromBody] AtualizarReceitaDto receitaDto)
        {
            var result = await _receitaServico.AtualizarReceita(id, receitaDto, GetUsuarioId());
            if (result.Sucesso)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<RespostaApi<bool>>> Delete(string id)
        {
            var result = await _receitaServico.RemoverReceita(id, GetUsuarioId());
            if (result.Sucesso)
            {
                return Ok(result);
            }
            return NotFound(result);
        }
    }
}
