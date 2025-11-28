using System.Collections.Generic;
using System.Threading.Tasks;
using ProjetoApiPT.Domain.Entities;

namespace ProjetoApiPT.Domain.Ports
{
    public interface IPropostaRepositorio
    {
    
        Task<Proposta> CriarAsync(Proposta proposta);

    
        Task<IEnumerable<Proposta>> ObterTodosAsync();

    
        Task<Proposta?> ObterPorIdAsync(int id);

    
        Task AtualizarAsync(Proposta proposta);

    
        Task ExcluirAsync(int id);
    }
}
