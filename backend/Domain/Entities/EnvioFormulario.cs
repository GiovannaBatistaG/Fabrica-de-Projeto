using System;

namespace ProjetoApiPT.Domain.Entities
{
    public class EnvioFormulario
    {
        public int Id { get; set; }
        public string NomeLead { get; set; } = null!;
        public string EmailContato { get; set; } = null!;
        public string StatusEnvio { get; set; } = "Pendente";

        public int IdModelo { get; set; } 
        
        public string DadosFormularioJson { get; set; } = null!;
        public DateTime DataEnvio { get; set; } = DateTime.UtcNow;

        public Modelo Modelo { get; set; } = null!; 
    }
}