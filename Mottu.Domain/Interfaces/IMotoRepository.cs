using Mottu.Domain.Entities;

namespace Mottu.Domain.Interfaces
{
    public interface IMotoRepository
    {
        Task<Moto> AdicionarAsync(Moto moto);
        Task<IEnumerable<Moto>> ObterTodosAsync();
        Task<IEnumerable<Moto>> ObterPorPlacaAsync(string placa);
        Task<Moto?> ObterPorIdAsync(string id);
        Task<Moto?> AtualizarAsync(Moto moto);
        Task<bool> RemoverAsync(string id);
        Task<bool> ExistePlacaAsync(string placa);
        Task<bool> PossuiLocacoesAsync(string motoId);
    }
}

