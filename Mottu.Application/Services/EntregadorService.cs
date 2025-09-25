using Mottu.Application.DTOs;
using Mottu.Application.Interfaces;
using Mottu.Domain.Entities;
using Mottu.Domain.Interfaces;

namespace Mottu.Application.Services
{
    public class EntregadorService : IEntregadorService
    {
        private readonly IEntregadorRepository _entregadorRepository;

        public EntregadorService(IEntregadorRepository entregadorRepository)
        {
            _entregadorRepository = entregadorRepository;
        }

        public async Task<EntregadorDto> CriarEntregadorAsync(CriarEntregadorDto criarEntregadorDto)
        {
            // Validar se o CNPJ já existe
            if (await _entregadorRepository.ExisteCnpjAsync(criarEntregadorDto.Cnpj))
                throw new ArgumentException("Já existe um entregador com este CNPJ");

            // Validar se o número da CNH já existe
            if (await _entregadorRepository.ExisteCnhAsync(criarEntregadorDto.NumeroCnh))
                throw new ArgumentException("Já existe um entregador com este número de CNH");

            var entregador = new Entregador
            {
                Identificador = criarEntregadorDto.Identificador,
                Nome = criarEntregadorDto.Nome,
                Cnpj = criarEntregadorDto.Cnpj,
                DataNascimento = criarEntregadorDto.DataNascimento,
                NumeroCnh = criarEntregadorDto.NumeroCnh,
                TipoCnh = criarEntregadorDto.TipoCnh,
                ImagemCnh = criarEntregadorDto.ImagemCnh,
                DataCriacao = DateTime.UtcNow
            };

            var entregadorCriado = await _entregadorRepository.AdicionarAsync(entregador);

            return new EntregadorDto
            {
                Identificador = entregadorCriado.Identificador,
                Nome = entregadorCriado.Nome,
                Cnpj = entregadorCriado.Cnpj,
                DataNascimento = entregadorCriado.DataNascimento,
                NumeroCnh = entregadorCriado.NumeroCnh,
                TipoCnh = entregadorCriado.TipoCnh,
                ImagemCnh = entregadorCriado.ImagemCnh
            };
        }

        public async Task<EntregadorDto?> ConsultarEntregadorPorIdAsync(string id)
        {
            var entregador = await _entregadorRepository.ObterPorIdAsync(id);
            if (entregador == null)
                return null;

            return new EntregadorDto
            {
                Identificador = entregador.Identificador,
                Nome = entregador.Nome,
                Cnpj = entregador.Cnpj,
                DataNascimento = entregador.DataNascimento,
                NumeroCnh = entregador.NumeroCnh,
                TipoCnh = entregador.TipoCnh,
                ImagemCnh = entregador.ImagemCnh
            };
        }

        public async Task<bool> AtualizarImagemCnhAsync(string id, string caminhoArquivo)
        {
            var entregador = await _entregadorRepository.ObterPorIdAsync(id);
            if (entregador == null)
                return false;

            // Atualizar entregador com o caminho da imagem
            entregador.ImagemCnh = caminhoArquivo;
            await _entregadorRepository.AtualizarAsync(entregador);

            return true;
        }
    }
}

