using System;

namespace ProjetoApiPT.Domain.Entities
{

    public class Usuario
    {

        public int Id { get; set; }

        public string NomeUsuario { get; set; } = null!;

        public string Email { get; set; } = null!;
        public string HashSenha { get; set; } = null!;

        public string PerfilAcesso { get; set; } = "User";

        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    }
}
