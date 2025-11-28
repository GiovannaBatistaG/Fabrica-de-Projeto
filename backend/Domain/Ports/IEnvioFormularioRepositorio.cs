using System.Collections.Generic;
using System.Threading.Tasks;
using ProjetoApiPT.Domain.Entities;

namespace ProjetoApiPT.Domain.Ports
{

    public interface IEnvioFormularioRepositorio
    {
         Task<EnvioFormulario> CriarAsync(EnvioFormulario envio);

        Task<EnvioFormulario?> ObterPorIdAsync(int id);

        Task<IEnumerable<EnvioFormulario>> ListarAsync();
        Task AtualizarAsync(EnvioFormulario envio);

         Task ExcluirAsync(int id);
    }
}
