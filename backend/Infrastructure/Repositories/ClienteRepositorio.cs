using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjetoApiPT.Domain.Entities;
using ProjetoApiPT.Domain.Ports;
using ProjetoApiPT.Infrastructure.Data;
using System; 

namespace ProjetoApiPT.Infrastructure.Repositories
{
    public class ClienteRepositorio : IClienteRepositorio
    {
        private readonly AppDbContext _contexto;
        private DbSet<Cliente> Clientes => _contexto.Clientes!; // Uso seguro do operador '!' no atalho

        public ClienteRepositorio(AppDbContext contexto)
        {
            _contexto = contexto ?? throw new ArgumentNullException(nameof(contexto));
        }

        public async Task<Cliente> CriarAsync(Cliente cliente)
        {
            await Clientes.AddAsync(cliente);
            await _contexto.SaveChangesAsync();
            return cliente;
        }

        public async Task<IEnumerable<Cliente>> ObterTodosAsync()
        {
            return await Clientes
                                  .AsNoTracking()
                                  .ToListAsync();
        }

        public async Task<Cliente?> ObterPorIdAsync(int id)
        {
            return await Clientes.FindAsync(id);
        }

        public async Task AtualizarAsync(Cliente cliente)
        {
            Clientes.Update(cliente);
            await _contexto.SaveChangesAsync();
        }

        public async Task ExcluirAsync(int id)
        {
            var cliente = await Clientes.FindAsync(id);
            
            if (cliente is not null)
            {
                Clientes.Remove(cliente);
                await _contexto.SaveChangesAsync();
            }
        }
    }
}