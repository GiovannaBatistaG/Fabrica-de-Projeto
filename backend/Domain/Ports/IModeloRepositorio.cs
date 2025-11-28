using System.Collections.Generic;
using System.Threading.Tasks;
using ProjetoApiPT.Domain.Entities;

namespace ProjetoApiPT.Domain.Ports
{
    public interface IModeloRepositorio
    {
        Task<Modelo> CriarAsync(Modelo modelo);
        Task<IEnumerable<Modelo>> ObterTodosAsync();

        Task<Modelo?> ObterPorIdAsync(int id);

        Task AtualizarAsync(Modelo modelo);

        Task ExcluirAsync(int id);
    }
}
