using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjetoApiPT.Domain.Entities;
using ProjetoApiPT.Domain.Ports;
using ProjetoApiPT.Infrastructure.Data;
using System; 

namespace ProjetoApiPT.Infrastructure.Repositories
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly AppDbContext _contexto;
        private DbSet<Usuario> Usuarios => _contexto.Usuarios!; // Uso seguro do operador '!' no atalho

        public UsuarioRepositorio(AppDbContext contexto)
        {
            _contexto = contexto ?? throw new ArgumentNullException(nameof(contexto));
        }

        public async Task<Usuario?> ObterPorIdAsync(int id)
        {
            return await Usuarios.FindAsync(id);
        }

        public async Task<Usuario?> ObterPorNomeUsuarioAsync(string nomeUsuario)
        {
            return await Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.NomeUsuario == nomeUsuario);
        }

        public async Task<Usuario?> ObterPorEmailAsync(string email)
        {
            return await Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task CriarAsync(Usuario usuario)
        {
            await Usuarios.AddAsync(usuario);
            await _contexto.SaveChangesAsync();
        }

        public async Task<IEnumerable<Usuario>> ObterTodosAsync()
        {
            return await Usuarios
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AtualizarAsync(Usuario usuario)
        {
            Usuarios.Update(usuario);
            await _contexto.SaveChangesAsync();
        }

        public async Task ExcluirAsync(int id)
        {
            var usuario = await Usuarios.FindAsync(id);
            
            if (usuario is not null)
            {
                Usuarios.Remove(usuario);
                await _contexto.SaveChangesAsync();
            }
        }
    }
}