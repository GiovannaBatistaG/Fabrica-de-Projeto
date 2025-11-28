using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjetoApiPT.Domain.Entities;
using ProjetoApiPT.Domain.Ports;

namespace ProjetoApiPT.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly IClienteRepositorio _repositorio;

    public ClientesController(IClienteRepositorio repositorio)
    {
       
        _repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
    }

    
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Cliente>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Cliente>>> ObterTodos()
    {
        var clientes = await _repositorio.ObterTodosAsync();
        return Ok(clientes);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Cliente), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Cliente>> ObterPorId(int id)
    {
        var cliente = await _repositorio.ObterPorIdAsync(id);
        
        // Trata o caso de nulo (NotFound)
        if (cliente is null)
            return NotFound(new { mensagem = "Cliente não encontrado." });
        
        return Ok(cliente);
    }


    [HttpPost]
    [ProducesResponseType(typeof(Cliente), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Cliente>> Criar([FromBody] Cliente cliente)
    {

        if (string.IsNullOrWhiteSpace(cliente.Nome) || 
            string.IsNullOrWhiteSpace(cliente.Email) || 
            string.IsNullOrWhiteSpace(cliente.Mensagem))
            return BadRequest(new { mensagem = "Nome, Email e Mensagem são campos obrigatórios." });
        
        // Chamada ao repositório para criar
        var clienteCriado = await _repositorio.CriarAsync(cliente);
        
        return CreatedAtAction(nameof(ObterPorId), new { id = clienteCriado.Id }, clienteCriado);
    }


    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] Cliente clienteAtualizado)
    {
        // 1. Verifica se o Id da rota corresponde ao Id no corpo (se fornecido)
        if (clienteAtualizado.Id != 0 && clienteAtualizado.Id != id)
            return BadRequest(new { mensagem = "O Id do Cliente não corresponde ao Id da rota." });
        

        var clienteExistente = await _repositorio.ObterPorIdAsync(id);
        
        if (clienteExistente is null)
            return NotFound(new { mensagem = "Cliente não encontrado." });
        

        clienteExistente.Nome = string.IsNullOrWhiteSpace(clienteAtualizado.Nome) ? clienteExistente.Nome : clienteAtualizado.Nome;
        clienteExistente.Email = string.IsNullOrWhiteSpace(clienteAtualizado.Email) ? clienteExistente.Email : clienteAtualizado.Email;
        clienteExistente.Mensagem = string.IsNullOrWhiteSpace(clienteAtualizado.Mensagem) ? clienteExistente.Mensagem : clienteAtualizado.Mensagem;

        // Propriedades podem ser copiadas diretamente quando não-nulas
        clienteExistente.Telefone = string.IsNullOrWhiteSpace(clienteAtualizado.Telefone) ? clienteExistente.Telefone : clienteAtualizado.Telefone;
        clienteExistente.PdfGerado = clienteAtualizado.PdfGerado;
        // A DataCadastro geralmente não é atualizada aqui.

        // 4. Chama o repositório para persistir a atualização
        await _repositorio.AtualizarAsync(clienteExistente);
        
        return NoContent();
    }


    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Excluir(int id)
    {
        // Busca o cliente existente apenas para verificar se existe (melhor UX)
        var cliente = await _repositorio.ObterPorIdAsync(id);
        
        if (cliente is null)
            return NotFound(new { mensagem = "Cliente não encontrado." });
        
        // Chama o repositório para excluir
        await _repositorio.ExcluirAsync(id);
        
        return NoContent();
    }
}