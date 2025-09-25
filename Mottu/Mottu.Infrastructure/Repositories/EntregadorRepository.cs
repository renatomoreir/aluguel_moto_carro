using Microsoft.EntityFrameworkCore;
using Mottu.Domain.Entities;
using Mottu.Domain.Interfaces;
using Mottu.Infrastructure.Data;

namespace Mottu.Infrastructure.Repositories
{
    public class EntregadorRepository : IEntregadorRepository
    {
        private readonly MottuDbContext _context;

        public EntregadorRepository(MottuDbContext context)
        {
            _context = context;
        }

        public async Task<Entregador> AdicionarAsync(Entregador entregador)
        {
            _context.Entregadores.Add(entregador);
            await _context.SaveChangesAsync();
            return entregador;
        }

        public async Task<Entregador?> ObterPorIdAsync(string id)
        {
            return await _context.Entregadores.FindAsync(id);
        }

        public async Task<Entregador?> AtualizarAsync(Entregador entregador)
        {
            _context.Entry(entregador).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entregador;
        }

        public async Task<bool> ExisteCnpjAsync(string cnpj)
        {
            return await _context.Entregadores.AnyAsync(e => e.Cnpj == cnpj);
        }

        public async Task<bool> ExisteCnhAsync(string numeroCnh)
        {
            return await _context.Entregadores.AnyAsync(e => e.NumeroCnh == numeroCnh);
        }
    }
}

