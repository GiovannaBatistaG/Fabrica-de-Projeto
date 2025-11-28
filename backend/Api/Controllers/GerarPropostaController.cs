using Microsoft.AspNetCore.Mvc;

namespace ProjetoApiPT.Api.Controllers;

[ApiController]
[Route("gerar-proposta")]
public class GerarPropostaController : ControllerBase
{
    public class LeadInput
    {
        public int? id { get; set; }
        public string? clientName { get; set; }
        public string? email { get; set; }
        public string? phone { get; set; }
        public string? company { get; set; }
        public string? message { get; set; }
        public string? servico { get; set; }
        public string? data { get; set; }
        public string? promptExtra { get; set; }
    }

    [HttpPost]
    public IActionResult Post([FromBody] LeadInput lead)
    {
        // Gera um texto simples com base nas informações do lead
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"Proposta para: {lead.clientName ?? "(Nome não informado)"}");
        sb.AppendLine($"Empresa: {lead.company ?? "(Empresa)"}");
        sb.AppendLine($"Serviço solicitado: {lead.servico ?? "(Serviço)"}");
        sb.AppendLine($"Contato: {lead.email ?? ""} / {lead.phone ?? ""}");
        sb.AppendLine();
        if (!string.IsNullOrWhiteSpace(lead.message))
        {
            sb.AppendLine("Mensagem do Lead:");
            sb.AppendLine(lead.message);
            sb.AppendLine();
        }
        if (!string.IsNullOrWhiteSpace(lead.promptExtra))
        {
            sb.AppendLine("Instruções adicionais:");
            sb.AppendLine(lead.promptExtra);
            sb.AppendLine();
        }

        sb.AppendLine("--- FIM DA PROPOSTA ---");

        var slides = sb.ToString();

        // Retorna slides e sem pdfUrl (o frontend pode gerar localmente)
        return Ok(new { slides = slides, pdfUrl = (string?)null });
    }
}
