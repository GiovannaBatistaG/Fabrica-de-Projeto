using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjetoApiPT.Domain.Entities;
using ProjetoApiPT.Domain.Ports;
using ProjetoApiPT.Infrastructure.Data;
using System;

namespace ProjetoApiPT.Infrastructure.Repositories
{
    public class ModeloRepositorio : IModeloRepositorio
    {
        private readonly AppDbContext _contexto;
        private DbSet<Modelo> Modelos => _contexto.Modelos!; // Uso seguro do operador '!' no atalho

        public ModeloRepositorio(AppDbContext contexto)
        {
            _contexto = contexto ?? throw new ArgumentNullException(nameof(contexto));
        }

        public async Task<Modelo> CriarAsync(Modelo modelo)
        {
            await Modelos.AddAsync(modelo);
            await _contexto.SaveChangesAsync();
            return modelo;
        }

        public async Task<IEnumerable<Modelo>> ObterTodosAsync()
        {
            return await Modelos
                                  .AsNoTracking()
                                  .ToListAsync();
        }

        public async Task<Modelo?> ObterPorIdAsync(int id)
        {
            return await Modelos.FindAsync(id);
        }

        public async Task AtualizarAsync(Modelo modelo)
        {
            Modelos.Update(modelo);
            await _contexto.SaveChangesAsync();
        }

        public async Task ExcluirAsync(int id)
        {
            var modelo = await Modelos.FindAsync(id); 

            if (modelo is not null)
            {
                Modelos.Remove(modelo);
                await _contexto.SaveChangesAsync();
            }
        }
    }
}