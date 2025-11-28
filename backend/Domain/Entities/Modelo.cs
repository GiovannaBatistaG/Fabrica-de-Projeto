using System;
using System.Collections.Generic; 

namespace ProjetoApiPT.Domain.Entities
{
    public class Modelo
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = null!;
        public string? Descricao { get; set; }
        public string Plano { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;


        public ICollection<EnvioFormulario> EnviosFormularios { get; set; } = new List<EnvioFormulario>();


        public ICollection<ProcessoModelo> ProcessosModelos { get; set; } = new List<ProcessoModelo>();
    }
}