using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ProjetoApiPT.Domain.Ports;
using ProjetoApiPT.Domain.Entities;
using System;
using System.Linq;

namespace ProjetoApiPT.Api.Controllers;

[ApiController]
[Route("api/propostas-cliente")] // <-- MUDANÇA: Rota nova e sem ambiguidade
public class ClientePropostaController : ControllerBase
{
    private readonly IClienteRepositorio _clienteRepositorio;
    private readonly ProjetoApiPT.Domain.Ports.IPropostaRepositorio _propostaRepositorio;

    public ClientePropostaController(IClienteRepositorio clienteRepositorio, ProjetoApiPT.Domain.Ports.IPropostaRepositorio propostaRepositorio)
    {
        _clienteRepositorio = clienteRepositorio;
        _propostaRepositorio = propostaRepositorio;
    }

    private static LeadDto MapClienteToLead(Cliente c)
    {
        return new LeadDto
        {
            Id = c.Id,
            ClientName = c.Nome,
            Email = c.Email,
            Phone = "", // Campo mantido para compatibilidade de contrato
            Company = c.Nome,
            Data = c.DataCadastro.ToString("yyyy-MM-dd")
        };
    }

    [HttpGet("leads-compat")] // Rota de compatibilidade
    [ProducesResponseType(typeof(IEnumerable<LeadDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        Console.WriteLine("[ClientePropostaController] GET /leads-compat chamado");
        var clientes = await _clienteRepositorio.ObterTodosAsync();
        var list = clientes.Select(MapClienteToLead).ToList();
        
        Console.WriteLine($"[ClientePropostaController] Retornando {list.Count()} leads");
        return Ok(list);
    }

    [HttpGet("leads-compat/{id}")] // Rota de compatibilidade
    [ProducesResponseType(typeof(LeadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var cliente = await _clienteRepositorio.ObterPorIdAsync(id);
        if (cliente is null)
        {
            return NotFound(new { message = "Lead not found" });
        }

        return Ok(MapClienteToLead(cliente));
    }

    [HttpPost("{clienteId}/criar")] // <-- MUDANÇA: Rota mais clara e RESTful
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CriarPropostaParaLead(int clienteId, [FromBody] CriarPropostaRequest request)
    {
        Console.WriteLine($"[ClientePropostaController] POST /api/propostas-cliente/{clienteId}/criar chamado");
        Console.WriteLine($"[ClientePropostaController] Request recebido.");
        
        var cliente = await _clienteRepositorio.ObterPorIdAsync(clienteId);
        if (cliente is null)
        {
            Console.WriteLine($"[ClientePropostaController] Cliente com ID {clienteId} não encontrado");
            return NotFound(new { message = "Cliente não encontrado" });
        }

        Console.WriteLine($"[ClientePropostaController] Cliente encontrado: {cliente.Nome}");

        try
        {
            var slides = request.Slides;
            var pdfUrl = request.PdfUrl;
            var statusValidacao = request.StatusValidacao;
            
            Console.WriteLine($"[ClientePropostaController] Slides: {(string.IsNullOrEmpty(slides) ? "vazio" : "preenchido")}");
            Console.WriteLine($"[ClientePropostaController] PdfUrl: {(string.IsNullOrEmpty(pdfUrl) ? "vazio" : "preenchido")}");
            Console.WriteLine($"[ClientePropostaController] StatusValidacao: {statusValidacao ?? "não especificado"}");

            // Se status não foi especificado, define como "Aguardando Validação" (padrão para envio para validação)
            var status = !string.IsNullOrWhiteSpace(statusValidacao) 
                ? statusValidacao 
                : "Aguardando Validação";

            var proposta = new ProjetoApiPT.Domain.Entities.Proposta
            {
                ClienteId = cliente.Id,
                NomeCliente = cliente.Nome,
                EmailCliente = cliente.Email,
                DataProposta = System.DateTime.UtcNow,
                StatusValidacao = status,
                MensagemEquipe = null,
                Valor = 0m,
                Responsavel = null,
                Slides = slides,
                PdfUrl = pdfUrl
            };

            var criada = await _propostaRepositorio.CriarAsync(proposta);
            
            Console.WriteLine($"[ClientePropostaController] Proposta criada com sucesso. ID: {criada.Id}");
            
            // Retorna a proposta criada com todos os dados
            var resposta = new 
            { 
                id = criada.Id,
                clienteId = criada.ClienteId,
                nomeCliente = criada.NomeCliente,
                emailCliente = criada.EmailCliente,
                dataProposta = criada.DataProposta,
                statusValidacao = criada.StatusValidacao,
                slides = criada.Slides,
                pdfUrl = criada.PdfUrl,
                valor = criada.Valor,
                mensagemEquipe = criada.MensagemEquipe,
                responsavel = criada.Responsavel,
                dataCriacao = criada.DataCriacao
            };
            
            Console.WriteLine($"[ClientePropostaController] Retornando resposta 200 OK");
            return Ok(resposta);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"[ClientePropostaController] ERRO ao criar proposta: {ex.Message}");
            Console.WriteLine($"[ClientePropostaController] StackTrace: {ex.StackTrace}");
            return StatusCode(500, new { message = "Erro ao criar proposta", detail = ex.Message });
        }
    }

    [HttpGet("{clienteId}/listar")]
    public async Task<IActionResult> ObterPropostasDoLead(int clienteId)
    {
        var cliente = await _clienteRepositorio.ObterPorIdAsync(clienteId);
        if (cliente is null) return NotFound(new { message = "Cliente não encontrado" });

        var todas = await _propostaRepositorio.ObterTodosAsync();
        var filtradas = System.Linq.Enumerable.Where(todas, p => p.ClienteId == clienteId);
        return Ok(filtradas);
    }
}
