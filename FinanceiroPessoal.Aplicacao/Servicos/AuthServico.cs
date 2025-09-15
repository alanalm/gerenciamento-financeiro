using FinanceiroPessoal.Aplicacao.DTOs;
using FinanceiroPessoal.Aplicacao.Interfaces;
using FinanceiroPessoal.Dominio.Comum;
using FinanceiroPessoal.Dominio.Entidades;
using FinanceiroPessoal.Dominio.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FinanceiroPessoal.Aplicacao.Servicos
{
    public class AuthServico : IAuthServico
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IConfiguration _config;

        public AuthServico(IUsuarioRepositorio usuarioRepositorio, IConfiguration config)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _config = config;
        }

        public async Task<RespostaApi<LoginRespostaDto>> LoginAsync(string email, string senha)
        {
            var usuario = await _usuarioRepositorio.ObterPorEmailAsync(email);
            if (usuario == null)
                return RespostaApi<LoginRespostaDto>.ErroResposta("Usuário não encontrado");

            if (!BCrypt.Net.BCrypt.Verify(senha, usuario.SenhaHash))
                return RespostaApi<LoginRespostaDto>.ErroResposta("Senha inválida");

            var token = GerarToken(usuario);

            var resposta = new LoginRespostaDto
            {
                Token = token,
                Nome = usuario.Nome ?? usuario.Email,
                Role = usuario.Role
            };

            return RespostaApi<LoginRespostaDto>.SucessoResposta(resposta, "Login realizado com sucesso");
        }

        public async Task<RespostaApi<UsuarioDto>> RegistrarAsync(UsuarioDto usuarioDto, string senha)
        {
            var existente = await _usuarioRepositorio.ObterPorEmailAsync(usuarioDto.Email);
            if (existente != null)
                return RespostaApi<UsuarioDto>.ErroResposta("E-mail já está em uso");

            var usuario = new Usuario(usuarioDto.Email, BCrypt.Net.BCrypt.HashPassword(senha))
            {
                Nome = usuarioDto.Nome,
                Role = usuarioDto.Role
            };

            await _usuarioRepositorio.CriarAsync(usuario);

            var retorno = new UsuarioDto
            {
                Id = usuario.Id,
                Email = usuario.Email,
                Nome = usuario.Nome,
                Role = usuario.Role
            };

            return RespostaApi<UsuarioDto>.SucessoResposta(retorno, "Usuário registrado com sucesso");
        }

        private string GerarToken(Usuario usuario)
        {
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];

            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
        new Claim(ClaimTypes.Email, usuario.Email),
        new Claim(ClaimTypes.Name, usuario.Nome),
        new Claim(ClaimTypes.Role, usuario.Role)
    };

            var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
