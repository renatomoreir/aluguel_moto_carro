using Mottu.Application.DTOs;

namespace Mottu.Application.Interfaces
{
    public interface IEntregadorService
    {
        Task<EntregadorDto> CriarEntregadorAsync(CriarEntregadorDto criarEntregadorDto);
        Task<EntregadorDto?> ConsultarEntregadorPorIdAsync(string id);
        Task<bool> AtualizarImagemCnhAsync(string id, string caminhoArquivo);
    }
}

