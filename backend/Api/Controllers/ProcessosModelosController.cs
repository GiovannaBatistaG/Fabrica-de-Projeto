using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjetoApiPT.Domain.Entities;
using ProjetoApiPT.Domain.Ports;

namespace ProjetoApiPT.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ProcessosModelosController : ControllerBase
    {
        /// Repositório de Processo de Modelo para acesso aos dados.
        private readonly IProcessoModeloRepositorio _repositorio;

        /// Inicializa uma nova instância do controlador ProcessosModelosController.
        
        public ProcessosModelosController(IProcessoModeloRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        /// Obtém todos os Processos de Modelos associados a um Modelo específico.
        
        [HttpGet("modelo/{idModelo}")]
        [ProducesResponseType(typeof(IEnumerable<ProcessoModelo>), 200)]
        public async Task<ActionResult<IEnumerable<ProcessoModelo>>> ObterPorIdModelo(int idModelo)
        {
            // Chama o repositório para obter os processos pelo Id do Modelo
            var processos = await _repositorio.ObterPorIdModeloAsync(idModelo);
            return Ok(processos);
        }


        /// Obtém um Processo de Modelo específico pelo seu Id.
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProcessoModelo), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ProcessoModelo>> ObterPorId(int id)
        {
            // Chama o repositório para obter o processo pelo Id
            var processo = await _repositorio.ObterPorIdAsync(id);
            
            // Se não encontrado, retorna 404
            if (processo == null)
                return NotFound(new { mensagem = "Processo de Modelo não encontrado." });
            
            return Ok(processo);
        }

        /// <param name="processo">Os dados do Processo de Modelo a ser criado.</param>
        [HttpPost]
        [ProducesResponseType(typeof(ProcessoModelo), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ProcessoModelo>> Criar([FromBody] ProcessoModelo processo)
        {
            // Valida se o objeto processo é nulo
            if (processo == null)
                return BadRequest(new { mensagem = "Os dados do Processo de Modelo são obrigatórios." });
            
            // Valida campos obrigatórios
            if (processo.IdModelo <= 0 || string.IsNullOrWhiteSpace(processo.DescricaoProcesso))
                return BadRequest(new { mensagem = "Id do Modelo e Descrição do Processo são campos obrigatórios." });
            
            // Chama o repositório para criar o processo
            var processoCriado = await _repositorio.CriarAsync(processo);
            
            // Retorna 201 Created com o processo criado
            return CreatedAtAction(nameof(ObterPorId), new { id = processoCriado.Id }, processoCriado);
        }

        /// Atualiza um Processo de Modelo existente.
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Atualizar(int id, [FromBody] ProcessoModelo processo)
        {
            // Valida se o objeto processo é nulo
            if (processo == null)
                return BadRequest(new { mensagem = "Os dados do Processo de Modelo são obrigatórios." });
            
            // Verifica se o Id no corpo da requisição corresponde ao Id da rota
            if (processo.Id != 0 && processo.Id != id)
                return BadRequest(new { mensagem = "O Id do Processo de Modelo não corresponde ao Id da rota." });
            
            // Busca o processo existente
            var processoExistente = await _repositorio.ObterPorIdAsync(id);
            if (processoExistente == null)
                return NotFound(new { mensagem = "Processo de Modelo não encontrado." });
            
            // Atualiza os dados do processo
            processoExistente.IdModelo = processo.IdModelo;
            processoExistente.DescricaoProcesso = processo.DescricaoProcesso ?? processoExistente.DescricaoProcesso;
            processoExistente.DataPrevista = processo.DataPrevista ?? processoExistente.DataPrevista;
            processoExistente.HoraPrevista = processo.HoraPrevista ?? processoExistente.HoraPrevista;
            
            // Chama o repositório para atualizar o processo
            await _repositorio.AtualizarAsync(processoExistente);
            
            // Retorna 204 No Content
            return NoContent();
        }

        /// Exclui um Processo de Modelo existente.

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Excluir(int id)
        {
            // Busca o processo existente
            var processo = await _repositorio.ObterPorIdAsync(id);
            if (processo == null)
                return NotFound(new { mensagem = "Processo de Modelo não encontrado." });
            
            // Chama o repositório para excluir o processo
            await _repositorio.ExcluirAsync(id);
            
            // Retorna 204 No Content
            return NoContent();
        }
    }
}
