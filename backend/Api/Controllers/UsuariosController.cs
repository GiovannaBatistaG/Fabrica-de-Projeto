using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjetoApiPT.Domain.Entities;
using ProjetoApiPT.Domain.Ports;

namespace ProjetoApiPT.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {

        private readonly IUsuarioRepositorio _repositorio;

      
        public UsuariosController(IUsuarioRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        /// <summary>
        /// Obtém todos os Usuários cadastrados.
        /// </summary>
        /// <returns>Uma lista de todos os Usuários.</returns>
        /// <response code="200">Retorna a lista de Usuários.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Usuario>), 200)]
        public async Task<ActionResult<IEnumerable<Usuario>>> ObterTodos()
        {
            // Chama o repositório para obter todos os usuários
            var usuarios = await _repositorio.ObterTodosAsync();
            return Ok(usuarios);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Usuario), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Usuario>> ObterPorId(int id)
        {
            // Chama o repositório para obter o usuário pelo Id
            var usuario = await _repositorio.ObterPorIdAsync(id);
            
            // Se não encontrado, retorna 404
            if (usuario == null)
                return NotFound(new { mensagem = "Usuário não encontrado." });
            
            return Ok(usuario);
        }


        [HttpGet("nome/{nomeUsuario}")]
        [ProducesResponseType(typeof(Usuario), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Usuario>> ObterPorNomeUsuario(string nomeUsuario)
        {
            // Chama o repositório para obter o usuário pelo nome de usuário
            var usuario = await _repositorio.ObterPorNomeUsuarioAsync(nomeUsuario);
            
            // Se não encontrado, retorna 404
            if (usuario == null)
                return NotFound(new { mensagem = "Usuário não encontrado." });
            
            return Ok(usuario);
        }


        [HttpGet("email/{email}")]
        [ProducesResponseType(typeof(Usuario), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Usuario>> ObterPorEmail(string email)
        {
            // Chama o repositório para obter o usuário pelo e-mail
            var usuario = await _repositorio.ObterPorEmailAsync(email);
            
            // Se não encontrado, retorna 404
            if (usuario == null)
                return NotFound(new { mensagem = "Usuário não encontrado." });
            
            return Ok(usuario);
        }


        [HttpPost]
        [ProducesResponseType(typeof(Usuario), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Usuario>> Criar([FromBody] Usuario usuario)
        {
            // Valida se o objeto usuário é nulo
            if (usuario == null)
                return BadRequest(new { mensagem = "Os dados do Usuário são obrigatórios." });
            
            // Valida campos obrigatórios
            if (string.IsNullOrWhiteSpace(usuario.NomeUsuario) || 
                string.IsNullOrWhiteSpace(usuario.Email) || 
                string.IsNullOrWhiteSpace(usuario.HashSenha))
                return BadRequest(new { mensagem = "Nome de Usuário, Email e Senha são campos obrigatórios." });
            
            // Verifica se o usuário já existe pelo nome de usuário
            var usuarioExistente = await _repositorio.ObterPorNomeUsuarioAsync(usuario.NomeUsuario);
            if (usuarioExistente != null)
                return BadRequest(new { mensagem = "Já existe um usuário com este nome de usuário." });
            
            // Verifica se o usuário já existe pelo e-mail
            var usuarioPorEmail = await _repositorio.ObterPorEmailAsync(usuario.Email);
            if (usuarioPorEmail != null)
                return BadRequest(new { mensagem = "Já existe um usuário com este e-mail." });
            
            // Chama o repositório para criar o usuário
            await _repositorio.CriarAsync(usuario);
            
            // Retorna 201 Created com o usuário criado
            return CreatedAtAction(nameof(ObterPorId), new { id = usuario.Id }, usuario);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Atualizar(int id, [FromBody] Usuario usuario)
        {
            // Valida se o objeto usuário é nulo
            if (usuario == null)
                return BadRequest(new { mensagem = "Os dados do Usuário são obrigatórios." });
            
            // Verifica se o Id no corpo da requisição corresponde ao Id da rota
            if (usuario.Id != 0 && usuario.Id != id)
                return BadRequest(new { mensagem = "O Id do Usuário não corresponde ao Id da rota." });
            
            // Busca o usuário existente
            var usuarioExistente = await _repositorio.ObterPorIdAsync(id);
            if (usuarioExistente == null)
                return NotFound(new { mensagem = "Usuário não encontrado." });
            
            // Atualiza os dados do usuário
            usuarioExistente.NomeUsuario = usuario.NomeUsuario ?? usuarioExistente.NomeUsuario;
            usuarioExistente.Email = usuario.Email ?? usuarioExistente.Email;
            usuarioExistente.HashSenha = usuario.HashSenha ?? usuarioExistente.HashSenha;
            usuarioExistente.PerfilAcesso = usuario.PerfilAcesso ?? usuarioExistente.PerfilAcesso;
            
            // Chama o repositório para atualizar o usuário
            await _repositorio.AtualizarAsync(usuarioExistente);
            
            // Retorna 204 No Content
            return NoContent();
        }

   
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Excluir(int id)
        {
            // Busca o usuário existente
            var usuario = await _repositorio.ObterPorIdAsync(id);
            if (usuario == null)
                return NotFound(new { mensagem = "Usuário não encontrado." });
            
            // Chama o repositório para excluir o usuário
            await _repositorio.ExcluirAsync(id);
            
            // Retorna 204 No Content
            return NoContent();
        }
    }
}
