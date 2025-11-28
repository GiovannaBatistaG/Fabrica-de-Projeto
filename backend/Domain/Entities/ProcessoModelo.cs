using System;

namespace ProjetoApiPT.Domain.Entities
{

    public class ProcessoModelo
    {
      
        public int Id { get; set; }


        public int IdModelo { get; set; }
        public string DescricaoProcesso { get; set; } = null!;

        public DateTime? DataPrevista { get; set; }

        public TimeSpan? HoraPrevista { get; set; }

        public Modelo? Modelo { get; set; }
    }
}
