using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProjetoApi.Application.DTOs;
using ProjetoApi.Application.Ports.Input;
using ProjetoApiPT.Domain.Entities;
using ProjetoApiPT.Domain.Ports;
using BC = BCrypt.Net.BCrypt;

namespace ProjetoApi.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IConfiguration _configuration;

        public AuthService(IUsuarioRepositorio usuarioRepositorio, IConfiguration configuration)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _configuration = configuration;
        }

        public async Task<LoginResultDTO> LoginAsync(LoginDTO loginDTO)
        {
            var usuario = await _usuarioRepositorio.ObterPorEmailAsync(loginDTO.Email);
            if (usuario == null)
            {
                throw new UnauthorizedAccessException("Email ou senha inválidos");
            }

            if (!BC.Verify(loginDTO.Senha, usuario.HashSenha))
            {
                throw new UnauthorizedAccessException("Email ou senha inválidos");
            }

            var token = GerarToken(usuario);

            return new LoginResultDTO
            {
                Token = token,
                Nome = usuario.NomeUsuario,
                Email = usuario.Email
            };
        }

        public async Task<LoginResultDTO> RegisterAsync(RegisterDTO registerDTO)
        {
            if (registerDTO.Senha != registerDTO.ConfirmarSenha)
            {
                throw new InvalidOperationException("As senhas não conferem");
            }

            var usuarioExistente = await _usuarioRepositorio.ObterPorEmailAsync(registerDTO.Email);
            if (usuarioExistente != null)
            {
                throw new InvalidOperationException("Email já está em uso");
            }

            var usuario = new Usuario
            {
                NomeUsuario = registerDTO.Nome,
                Email = registerDTO.Email,
                HashSenha = BC.HashPassword(registerDTO.Senha),
                PerfilAcesso = "User",
                DataCriacao = DateTime.UtcNow
            };

            await _usuarioRepositorio.CriarAsync(usuario);

            var token = GerarToken(usuario);

            return new LoginResultDTO
            {
                Token = token,
                Nome = usuario.NomeUsuario,
                Email = usuario.Email
            };
        }

        private string GerarToken(Usuario usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"] ?? throw new InvalidOperationException("JWT Secret não configurado"));
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Name, usuario.NomeUsuario),
                    new Claim(ClaimTypes.Email, usuario.Email),
                    new Claim(ClaimTypes.Role, usuario.PerfilAcesso)
                }),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
