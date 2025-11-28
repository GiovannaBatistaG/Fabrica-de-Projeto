using System;

namespace ProjetoApiPT.Domain.Entities
{
    public class Proposta
    {

        public int Id { get; set; }

        public string NomeCliente { get; set; } = null!;

        public string EmailCliente { get; set; } = null!;

        public DateTime DataProposta { get; set; }

        public int ClienteId { get; set; }
        public string? Slides { get; set; }

        public string? PdfUrl { get; set; }

        public string StatusValidacao { get; set; } = null!;
        public string? MensagemEquipe { get; set; }
        public decimal Valor { get; set; }
        public string? Responsavel { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    }
}
