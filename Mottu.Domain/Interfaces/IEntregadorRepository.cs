using Mottu.Domain.Entities;

namespace Mottu.Domain.Interfaces
{
    public interface IEntregadorRepository
    {
        Task<Entregador> AdicionarAsync(Entregador entregador);
        Task<Entregador?> ObterPorIdAsync(string id);
        Task<Entregador?> AtualizarAsync(Entregador entregador);
        Task<bool> ExisteCnpjAsync(string cnpj);
        Task<bool> ExisteCnhAsync(string numeroCnh);
    }
}

