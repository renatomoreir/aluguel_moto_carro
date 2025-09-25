using Mottu.Application.DTOs;

namespace Mottu.Application.Interfaces
{
    public interface IMotoService
    {
        Task<MotoDto> CriarMotoAsync(CriarMotoDto criarMotoDto);
        Task<IEnumerable<MotoDto>> ConsultarMotosAsync(string? placa = null);
        Task<MotoDto?> ConsultarMotoPorIdAsync(string id);
        Task<MotoDto?> AtualizarPlacaAsync(string id, string novaPlaca);
        Task<bool> RemoverMotoAsync(string id);
    }
}

