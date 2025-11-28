using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjetoApiPT.Domain.Entities;
using ProjetoApiPT.Domain.Ports;

namespace ProjetoApiPT.Api.Controllers
{


    [ApiController]
    [Route("api/[controller]")]
    public class EnviosFormulariosController : ControllerBase
    {


        private readonly IEnvioFormularioRepositorio _repositorio;


       
        public EnviosFormulariosController(IEnvioFormularioRepositorio repositorio)
        {
            _repositorio = repositorio;
        }


        /// Obtém todos os Envios de Formulários cadastrados.

        /// <response code="200">Retorna a lista de Envios de Formulários.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EnvioFormulario>), 200)]
        public async Task<ActionResult<IEnumerable<EnvioFormulario>>> ListarTodos()
        {
            // Chama o repositório para obter todos os envios de formulários
            var envios = await _repositorio.ListarAsync();
            return Ok(envios);
        }


        /// Obtém um Envio de Formulário específico pelo seu Id.

        /// <param name="id">O Id do Envio de Formulário a ser recuperado.</param>

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EnvioFormulario), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<EnvioFormulario>> ObterPorId(int id)
        {

            var envio = await _repositorio.ObterPorIdAsync(id);
            
            // Se não encontrado, retorna 404
            if (envio == null)
                return NotFound(new { mensagem = "Envio de Formulário não encontrado." });
            
            return Ok(envio);
        }

        /// Cria um novo Envio de Formulário.
        [HttpPost]
        [ProducesResponseType(typeof(EnvioFormulario), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<EnvioFormulario>> Criar([FromBody] EnvioFormulario envio)
        {

            if (envio == null)
                return BadRequest(new { mensagem = "Os dados do Envio de Formulário são obrigatórios." });
            

            if (string.IsNullOrWhiteSpace(envio.NomeLead) || 
                string.IsNullOrWhiteSpace(envio.EmailContato) || 
                string.IsNullOrWhiteSpace(envio.DadosFormularioJson))
                return BadRequest(new { mensagem = "Nome do Lead, Email de Contato e Dados do Formulário são campos obrigatórios." });
            
            // Chama o repositório para criar o envio
            var envioCriado = await _repositorio.CriarAsync(envio);
            
            // Retorna 201 Created com o envio criado
            return CreatedAtAction(nameof(ObterPorId), new { id = envioCriado.Id }, envioCriado);
        }


 
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Atualizar(int id, [FromBody] EnvioFormulario envio)
        {
            // Valida se o objeto envio é nulo
            if (envio == null)
                return BadRequest(new { mensagem = "Os dados do Envio de Formulário são obrigatórios." });
            
            // Verifica se o Id no corpo da requisição corresponde ao Id da rota
            if (envio.Id != 0 && envio.Id != id)
                return BadRequest(new { mensagem = "O Id do Envio de Formulário não corresponde ao Id da rota." });
            
            // Busca o envio existente
            var envioExistente = await _repositorio.ObterPorIdAsync(id);
            if (envioExistente == null)
                return NotFound(new { mensagem = "Envio de Formulário não encontrado." });
            
            // Atualiza os dados do envio
            envioExistente.NomeLead = envio.NomeLead ?? envioExistente.NomeLead;
            envioExistente.EmailContato = envio.EmailContato ?? envioExistente.EmailContato;
            envioExistente.StatusEnvio = envio.StatusEnvio ?? envioExistente.StatusEnvio;
            envioExistente.IdModelo = envio.IdModelo;
            envioExistente.DadosFormularioJson = envio.DadosFormularioJson ?? envioExistente.DadosFormularioJson;
            
            // Chama o repositório para atualizar o envio
            await _repositorio.AtualizarAsync(envioExistente);
            
            // Retorna 204 No Content
            return NoContent();
        }


        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Excluir(int id)
        {
            // Busca o envio existente
            var envio = await _repositorio.ObterPorIdAsync(id);
            if (envio == null)
                return NotFound(new { mensagem = "Envio de Formulário não encontrado." });
            
            // Chama o repositório para excluir o envio
            await _repositorio.ExcluirAsync(id);
            
            // Retorna 204 No Content
            return NoContent();
        }
    }
}
