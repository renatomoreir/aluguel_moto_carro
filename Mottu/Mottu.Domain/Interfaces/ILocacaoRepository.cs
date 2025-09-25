using Mottu.Domain.Entities;

namespace Mottu.Domain.Interfaces
{
    public interface ILocacaoRepository
    {
        Task<Locacao> AdicionarAsync(Locacao locacao);
        Task<Locacao?> ObterPorIdAsync(string id);
        Task<Locacao?> AtualizarAsync(Locacao locacao);
        Task<bool> EntregadorPossuiLocacaoAtivaAsync(string entregadorId);
        Task<bool> MotoPossuiLocacaoAtivaAsync(string motoId);
    }
}

