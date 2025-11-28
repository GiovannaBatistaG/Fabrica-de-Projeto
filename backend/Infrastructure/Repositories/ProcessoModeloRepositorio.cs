using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjetoApiPT.Domain.Entities;
using ProjetoApiPT.Domain.Ports;
using ProjetoApiPT.Infrastructure.Data;
using System; 

namespace ProjetoApiPT.Infrastructure.Repositories
{
    public class ProcessoModeloRepositorio : IProcessoModeloRepositorio
    {
        private readonly AppDbContext _contexto;
        private DbSet<ProcessoModelo> ProcessosModelos => _contexto.ProcessosModelos!; // Uso seguro do operador '!' no atalho

        public ProcessoModeloRepositorio(AppDbContext contexto)
        {
            _contexto = contexto ?? throw new ArgumentNullException(nameof(contexto));
        }

        public async Task<ProcessoModelo> CriarAsync(ProcessoModelo processo)
        {
            await ProcessosModelos.AddAsync(processo);
            await _contexto.SaveChangesAsync();
            return processo;
        }

        public async Task<IEnumerable<ProcessoModelo>> ObterPorIdModeloAsync(int idModelo)
        {
            return await ProcessosModelos
                .AsNoTracking()
                .Where(p => p.IdModelo == idModelo)
                .ToListAsync();
        }

        public async Task<ProcessoModelo?> ObterPorIdAsync(int id)
        {
            // Se precisar incluir o Modelo:
            return await ProcessosModelos
                .Include(p => p.Modelo) // Removido o '!' inseguro
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AtualizarAsync(ProcessoModelo processo)
        {
            ProcessosModelos.Update(processo);
            await _contexto.SaveChangesAsync();
        }

        public async Task ExcluirAsync(int id)
        {
            var processo = await ProcessosModelos.FindAsync(id); 
            
            if (processo is not null)
            {
                ProcessosModelos.Remove(processo);
                await _contexto.SaveChangesAsync();
            }
        }
    }
}