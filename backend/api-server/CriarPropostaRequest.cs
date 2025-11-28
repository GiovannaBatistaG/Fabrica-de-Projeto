namespace ProjetoApiPT.Api.Controllers;

public class CriarPropostaRequest
{
    public string? Slides { get; set; }
    public string? PdfUrl { get; set; }
    public string? StatusValidacao { get; set; }
}