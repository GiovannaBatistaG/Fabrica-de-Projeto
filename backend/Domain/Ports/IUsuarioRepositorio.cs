using System.Collections.Generic;
using System.Threading.Tasks;
using ProjetoApiPT.Domain.Entities;

namespace ProjetoApiPT.Domain.Ports
{
    public interface IUsuarioRepositorio
    {

        Task<Usuario?> ObterPorIdAsync(int id);
        Task<Usuario?> ObterPorNomeUsuarioAsync(string nomeUsuario);

        Task<Usuario?> ObterPorEmailAsync(string email);
        Task CriarAsync(Usuario usuario);
        Task<IEnumerable<Usuario>> ObterTodosAsync();
        Task AtualizarAsync(Usuario usuario);
        Task ExcluirAsync(int id);
    }
}
