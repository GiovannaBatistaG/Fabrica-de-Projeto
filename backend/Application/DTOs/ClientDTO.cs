namespace ProjetoApi.Application.DTOs
{
    public class ClientDTO
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public required string Email { get; set; }
        public required string Mensagem { get; set; }
        public string? Telefone { get; set; }
        public bool PdfGerado { get; set; }
        public DateTime DataCadastro { get; set; }
    }
}
