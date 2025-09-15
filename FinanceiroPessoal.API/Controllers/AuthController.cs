using FinanceiroPessoal.Aplicacao.DTOs;
using FinanceiroPessoal.Aplicacao.Interfaces;
using FinanceiroPessoal.Dominio.Comum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceiroPessoal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthServico _authServico;

    public AuthController(IAuthServico authServico)
    {
        _authServico = authServico;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RespostaApi<LoginRespostaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaApi<LoginRespostaDto>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var resposta = await _authServico.LoginAsync(dto.Email, dto.Senha);

        if (!resposta.Sucesso)
            return Unauthorized(resposta);

        return Ok(resposta);
    }

    [HttpPost("registrar")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RespostaApi<UsuarioDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaApi<UsuarioDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Registrar([FromBody] RegistrarUsuarioDto dto)
    {
        var usuarioDto = new UsuarioDto
        {
            Email = dto.Email,
            Nome = dto.Nome,
            Role = "Usuario"
        };

        var resposta = await _authServico.RegistrarAsync(usuarioDto, dto.Senha);

        if (!resposta.Sucesso)
            return BadRequest(resposta);

        return Ok(resposta);
    }
}
