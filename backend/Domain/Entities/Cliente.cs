using System;

namespace ProjetoApiPT.Domain.Entities
{

    public class Cliente
    {

        public int Id { get; set; }


        public string Nome { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Status { get; set; } = null!;

        public string? Telefone { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public bool PdfGerado { get; set; }
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
        public System.Collections.Generic.List<Proposta>? Propostas { get; set; }
    }
}
