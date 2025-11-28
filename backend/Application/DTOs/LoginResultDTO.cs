namespace ProjetoApi.Application.DTOs
{
    public class LoginResultDTO
    {
        public required string Token { get; set; }
        public required string Nome { get; set; }
        public required string Email { get; set; }
    }
}
