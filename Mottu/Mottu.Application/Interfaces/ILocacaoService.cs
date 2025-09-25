using Mottu.Application.DTOs;

namespace Mottu.Application.Interfaces
{
    public interface ILocacaoService
    {
        Task<LocacaoDto> CriarLocacaoAsync(CriarLocacaoDto criarLocacaoDto);
        Task<LocacaoDto?> ConsultarLocacaoPorIdAsync(string id);
        Task<CalculoLocacaoDto?> CalcularValorDevolucaoAsync(string id, DateTime dataDevolucao);
    }
}

