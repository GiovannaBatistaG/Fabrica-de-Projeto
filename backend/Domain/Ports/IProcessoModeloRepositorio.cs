using System.Collections.Generic;
using System.Threading.Tasks;
using ProjetoApiPT.Domain.Entities;

namespace ProjetoApiPT.Domain.Ports
{

    public interface IProcessoModeloRepositorio
    {

        Task<ProcessoModelo> CriarAsync(ProcessoModelo processo);

        Task<IEnumerable<ProcessoModelo>> ObterPorIdModeloAsync(int idModelo);

        Task<ProcessoModelo?> ObterPorIdAsync(int id);

        Task AtualizarAsync(ProcessoModelo processo);
        Task ExcluirAsync(int id);
    }
}
