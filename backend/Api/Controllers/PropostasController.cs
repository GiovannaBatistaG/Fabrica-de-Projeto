using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjetoApiPT.Domain.Entities;
using ProjetoApiPT.Domain.Ports;

namespace ProjetoApiPT.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class PropostasController : ControllerBase
    {

        private readonly IPropostaRepositorio _repositorio;

        public PropostasController(IPropostaRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Proposta>), 200)]
        public async Task<ActionResult<IEnumerable<Proposta>>> ObterTodas()
        {
            // Chama o repositório para obter todas as propostas
            var propostas = await _repositorio.ObterTodosAsync();
            return Ok(propostas);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Proposta), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Proposta>> ObterPorId(int id)
        {
            // Chama o repositório para obter a proposta pelo Id
            var proposta = await _repositorio.ObterPorIdAsync(id);
            
            // Se não encontrada, retorna 404
            if (proposta == null)
                return NotFound(new { mensagem = "Proposta não encontrada." });
            
            return Ok(proposta);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Proposta), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Proposta>> Criar([FromBody] Proposta proposta)
        {
            // Valida se o objeto proposta é nulo
            if (proposta == null)
                return BadRequest(new { mensagem = "Os dados da Proposta são obrigatórios." });
            
            // Valida campos obrigatórios
            if (string.IsNullOrWhiteSpace(proposta.NomeCliente) || 
                string.IsNullOrWhiteSpace(proposta.EmailCliente) || 
                string.IsNullOrWhiteSpace(proposta.StatusValidacao))
                return BadRequest(new { mensagem = "Nome do Cliente, Email do Cliente e Status de Validação são campos obrigatórios." });
            
            // Chama o repositório para criar a proposta
            var propostaCriada = await _repositorio.CriarAsync(proposta);
            
            // Retorna 201 Created com a proposta criada
            return CreatedAtAction(nameof(ObterPorId), new { id = propostaCriada.Id }, propostaCriada);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Atualizar(int id, [FromBody] Proposta proposta)
        {
            // Valida se o objeto proposta é nulo
            if (proposta == null)
                return BadRequest(new { mensagem = "Os dados da Proposta são obrigatórios." });
            
            // Verifica se o Id no corpo da requisição corresponde ao Id da rota
            if (proposta.Id != 0 && proposta.Id != id)
                return BadRequest(new { mensagem = "O Id da Proposta não corresponde ao Id da rota." });
            
            // Busca a proposta existente
            var propostaExistente = await _repositorio.ObterPorIdAsync(id);
            if (propostaExistente == null)
                return NotFound(new { mensagem = "Proposta não encontrada." });
            
            // Atualiza os dados da proposta
            propostaExistente.NomeCliente = proposta.NomeCliente ?? propostaExistente.NomeCliente;
            propostaExistente.EmailCliente = proposta.EmailCliente ?? propostaExistente.EmailCliente;
            propostaExistente.DataProposta = proposta.DataProposta;
            propostaExistente.StatusValidacao = proposta.StatusValidacao ?? propostaExistente.StatusValidacao;
            propostaExistente.MensagemEquipe = proposta.MensagemEquipe ?? propostaExistente.MensagemEquipe;
            propostaExistente.Valor = proposta.Valor;
            propostaExistente.Responsavel = proposta.Responsavel ?? propostaExistente.Responsavel;
            
            // Chama o repositório para atualizar a proposta
            await _repositorio.AtualizarAsync(propostaExistente);
            
            // Retorna 204 No Content
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Excluir(int id)
        {
            // Busca a proposta existente
            var proposta = await _repositorio.ObterPorIdAsync(id);
            if (proposta == null)
                return NotFound(new { mensagem = "Proposta não encontrada." });
            
            // Chama o repositório para excluir a proposta
            await _repositorio.ExcluirAsync(id);
            
            // Retorna 204 No Content
            return NoContent();
        }
    }
}
