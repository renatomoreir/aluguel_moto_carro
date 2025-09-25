using Microsoft.EntityFrameworkCore;
using Mottu.Domain.Entities;
using Mottu.Domain.Interfaces;
using Mottu.Infrastructure.Data;

namespace Mottu.Infrastructure.Repositories
{
    public class LocacaoRepository : ILocacaoRepository
    {
        private readonly MottuDbContext _context;

        public LocacaoRepository(MottuDbContext context)
        {
            _context = context;
        }

        public async Task<Locacao> AdicionarAsync(Locacao locacao)
        {
            _context.Locacoes.Add(locacao);
            await _context.SaveChangesAsync();
            return locacao;
        }

        public async Task<Locacao?> ObterPorIdAsync(string id)
        {
            return await _context.Locacoes
                .Include(l => l.Entregador)
                .Include(l => l.Moto)
                .FirstOrDefaultAsync(l => l.Identificador == id);
        }

        public async Task<Locacao?> AtualizarAsync(Locacao locacao)
        {
            _context.Entry(locacao).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return locacao;
        }

        public async Task<bool> EntregadorPossuiLocacaoAtivaAsync(string entregadorId)
        {
            return await _context.Locacoes
                .AnyAsync(l => l.EntregadorId == entregadorId && l.DataDevolucao == null);
        }

        public async Task<bool> MotoPossuiLocacaoAtivaAsync(string motoId)
        {
            return await _context.Locacoes
                .AnyAsync(l => l.MotoId == motoId && l.DataDevolucao == null);
        }
    }
}

