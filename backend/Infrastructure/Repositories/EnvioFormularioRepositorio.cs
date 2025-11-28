using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjetoApiPT.Domain.Entities;
using ProjetoApiPT.Domain.Ports;
using ProjetoApiPT.Infrastructure.Data;
using System; 

namespace ProjetoApiPT.Infrastructure.Repositories
{
    public class EnvioFormularioRepositorio : IEnvioFormularioRepositorio
    {
        private readonly AppDbContext _contexto;
        private DbSet<EnvioFormulario> EnviosFormularios => _contexto.EnviosFormularios!; // Uso seguro do operador '!' no atalho

        public EnvioFormularioRepositorio(AppDbContext contexto)
        {
            _contexto = contexto ?? throw new ArgumentNullException(nameof(contexto));
        }

        public async Task<EnvioFormulario> CriarAsync(EnvioFormulario envio)
        {
            await EnviosFormularios.AddAsync(envio);
            await _contexto.SaveChangesAsync();
            return envio;
        }

        public async Task<EnvioFormulario?> ObterPorIdAsync(int id)
        {
            return await EnviosFormularios.FindAsync(id);
        }

        public async Task<IEnumerable<EnvioFormulario>> ListarAsync()
        {
            return await EnviosFormularios
                                  .AsNoTracking()
                                  .ToListAsync();
        }

        public async Task AtualizarAsync(EnvioFormulario envio)
        {
            EnviosFormularios.Update(envio);
            await _contexto.SaveChangesAsync();
        }

        public async Task ExcluirAsync(int id)
        {
            var envio = await EnviosFormularios.FindAsync(id); 
            
            if (envio is not null)
            {
                EnviosFormularios.Remove(envio);
                await _contexto.SaveChangesAsync();
            }
        }
    }
}