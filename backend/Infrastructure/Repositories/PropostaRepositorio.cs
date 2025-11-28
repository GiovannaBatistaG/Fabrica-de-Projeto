using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjetoApiPT.Domain.Entities;
using ProjetoApiPT.Domain.Ports;
using ProjetoApiPT.Infrastructure.Data;
using System; 

namespace ProjetoApiPT.Infrastructure.Repositories
{
    public class PropostaRepositorio : IPropostaRepositorio
    {
        private readonly AppDbContext _contexto;
        // CORREÇÃO: Uso do operador '!' para resolver o CS8604, 
        // garantindo ao compilador que _contexto.Propostas não é nulo em tempo de execução.
        private DbSet<Proposta> Propostas => _contexto.Propostas!; 

        public PropostaRepositorio(AppDbContext contexto)
        {
            _contexto = contexto ?? throw new ArgumentNullException(nameof(contexto));
        }

        public async Task<Proposta> CriarAsync(Proposta proposta)
        {
            await Propostas.AddAsync(proposta);
            await _contexto.SaveChangesAsync();
            return proposta;
        }

        // --- Método ObterTodos ---
        public async Task<IEnumerable<Proposta>> ObterTodosAsync()
        {
            return await Propostas
                                  .AsNoTracking()
                                  .ToListAsync();
        }

        // --- Método ObterPorId ---
        public async Task<Proposta?> ObterPorIdAsync(int id)
        {
            return await Propostas.FindAsync(id);
        }

        public async Task AtualizarAsync(Proposta proposta)
        {
            Propostas.Update(proposta);
            await _contexto.SaveChangesAsync();
        }

        // --- Método Excluir ---
        public async Task ExcluirAsync(int id)
        {
            var proposta = await Propostas.FindAsync(id);
            
            if (proposta is not null)
            {
                Propostas.Remove(proposta);
                await _contexto.SaveChangesAsync();
            }
        }
    }
}