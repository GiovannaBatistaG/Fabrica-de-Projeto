using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjetoApiPT.Domain.Entities;
using ProjetoApiPT.Domain.Ports;

namespace ProjetoApiPT.Api.Controllers
{



    [ApiController]
    [Route("api/[controller]")]
    public class ModelosController : ControllerBase
    {
 
        /// Repositório de Modelo para acesso aos dados.
      
        private readonly IModeloRepositorio _repositorio;


        /// Inicializa uma nova instância do controlador ModelosController.

        public ModelosController(IModeloRepositorio repositorio)
        {
            _repositorio = repositorio;
        }



        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Modelo>), 200)]
        public async Task<ActionResult<IEnumerable<Modelo>>> ObterTodos()
        {
            // Chama o repositório para obter todos os modelos
            var modelos = await _repositorio.ObterTodosAsync();
            return Ok(modelos);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Modelo), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Modelo>> ObterPorId(int id)
        {
            // Chama o repositório para obter o modelo pelo Id
            var modelo = await _repositorio.ObterPorIdAsync(id);
            
            // Se não encontrado, retorna 404
            if (modelo == null)
                return NotFound(new { mensagem = "Modelo não encontrado." });
            
            return Ok(modelo);
        }


        /// Cria um novo Modelo.
        /// <param name="modelo">Os dados do Modelo a ser criado.</param>
        [HttpPost]
        [ProducesResponseType(typeof(Modelo), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Modelo>> Criar([FromBody] Modelo modelo)
        {
            // Valida se o objeto modelo é nulo
            if (modelo == null)
                return BadRequest(new { mensagem = "Os dados do Modelo são obrigatórios." });
            
            // Valida campos obrigatórios
            if (string.IsNullOrWhiteSpace(modelo.Titulo) || 
                string.IsNullOrWhiteSpace(modelo.Plano) || 
                string.IsNullOrWhiteSpace(modelo.Status))
                return BadRequest(new { mensagem = "Título, Plano e Status são campos obrigatórios." });
            
            // Chama o repositório para criar o modelo
            var modeloCriado = await _repositorio.CriarAsync(modelo);
            
            // Retorna 201 Created com o modelo criado
            return CreatedAtAction(nameof(ObterPorId), new { id = modeloCriado.Id }, modeloCriado);
        }


        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Atualizar(int id, [FromBody] Modelo modelo)
        {
            if (modelo == null)
                return BadRequest(new { mensagem = "Os dados do Modelo são obrigatórios." });

            if (modelo.Id != 0 && modelo.Id != id)
                return BadRequest(new { mensagem = "O Id do Modelo não corresponde ao Id da rota." });
            
            // Busca o modelo existente
            var modeloExistente = await _repositorio.ObterPorIdAsync(id);
            if (modeloExistente == null)
                return NotFound(new { mensagem = "Modelo não encontrado." });
            
            // Atualiza os dados do modelo
            modeloExistente.Titulo = modelo.Titulo ?? modeloExistente.Titulo;
            modeloExistente.Descricao = modelo.Descricao ?? modeloExistente.Descricao;
            modeloExistente.Plano = modelo.Plano ?? modeloExistente.Plano;
            modeloExistente.Status = modelo.Status ?? modeloExistente.Status;
            
            // Chama o repositório para atualizar o modelo
            await _repositorio.AtualizarAsync(modeloExistente);
            
            // Retorna 204 No Content
            return NoContent();
        }

        /// Exclui um Modelo existente.
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Excluir(int id)
        {
            // Busca o modelo existente
            var modelo = await _repositorio.ObterPorIdAsync(id);
            if (modelo == null)
                return NotFound(new { mensagem = "Modelo não encontrado." });
            
            // Chama o repositório para excluir o modelo
            await _repositorio.ExcluirAsync(id);
            
            // Retorna 204 No Content
            return NoContent();
        }
    }
}
