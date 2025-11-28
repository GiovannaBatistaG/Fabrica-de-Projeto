namespace ProjetoApiPT.Api.Controllers;

public class LeadDto
{
    public int Id { get; set; }
    public string? ClientName { get; set; }
    public string? Email { get; set; }
    public string Phone { get; set; } = "";
    public string? Company { get; set; }
    public string Data { get; set; } = "";
}