using Microsoft.EntityFrameworkCore;
using Mottu.Domain.Entities;
using Mottu.Domain.Interfaces;
using Mottu.Infrastructure.Data;

namespace Mottu.Infrastructure.Repositories
{
    public class MotoRepository : IMotoRepository
    {
        private readonly MottuDbContext _context;

        public MotoRepository(MottuDbContext context)
        {
            _context = context;
        }

        public async Task<Moto> AdicionarAsync(Moto moto)
        {
            _context.Motos.Add(moto);
            await _context.SaveChangesAsync();
            return moto;
        }

        public async Task<IEnumerable<Moto>> ObterTodosAsync()
        {
            return await _context.Motos.ToListAsync();
        }

        public async Task<IEnumerable<Moto>> ObterPorPlacaAsync(string placa)
        {
            return await _context.Motos
                .Where(m => m.Placa.Contains(placa))
                .ToListAsync();
        }

        public async Task<Moto?> ObterPorIdAsync(string id)
        {
            return await _context.Motos.FindAsync(id);
        }

        public async Task<Moto?> AtualizarAsync(Moto moto)
        {
            _context.Entry(moto).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return moto;
        }

        public async Task<bool> RemoverAsync(string id)
        {
            var moto = await _context.Motos.FindAsync(id);
            if (moto == null)
                return false;

            _context.Motos.Remove(moto);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistePlacaAsync(string placa)
        {
            return await _context.Motos.AnyAsync(m => m.Placa == placa);
        }

        public async Task<bool> PossuiLocacoesAsync(string motoId)
        {
            return await _context.Locacoes.AnyAsync(l => l.MotoId == motoId);
        }
    }
}

