using Mottu.Application.DTOs;
using Mottu.Application.Interfaces;
using Mottu.Domain.Entities;
using Mottu.Domain.Events;
using Mottu.Domain.Interfaces;

namespace Mottu.Application.Services
{
    public class MotoService : IMotoService
    {
        private readonly IMotoRepository _motoRepository;
        private readonly IMessagePublisher _messagePublisher;

        public MotoService(IMotoRepository motoRepository, IMessagePublisher messagePublisher)
        {
            _motoRepository = motoRepository;
            _messagePublisher = messagePublisher;
        }

        public async Task<MotoDto> CriarMotoAsync(CriarMotoDto criarMotoDto)
        {
            // Validar se a placa já existe
            if (await _motoRepository.ExistePlacaAsync(criarMotoDto.Placa))
                throw new ArgumentException("Já existe uma moto com esta placa");

            var moto = new Moto
            {
                Identificador = criarMotoDto.Identificador,
                Ano = criarMotoDto.Ano,
                Modelo = criarMotoDto.Modelo,
                Placa = criarMotoDto.Placa,
                DataCriacao = DateTime.UtcNow
            };

            var motoCriada = await _motoRepository.AdicionarAsync(moto);

            // Publicar evento de moto cadastrada via RabbitMQ
            var evento = new MotoCadastradaEvent
            {
                Identificador = motoCriada.Identificador,
                Ano = motoCriada.Ano,
                Modelo = motoCriada.Modelo,
                Placa = motoCriada.Placa,
                DataCadastro = motoCriada.DataCriacao
            };

            await _messagePublisher.PublishAsync(evento, "moto.cadastrada");

            return new MotoDto
            {
                Identificador = motoCriada.Identificador,
                Ano = motoCriada.Ano,
                Modelo = motoCriada.Modelo,
                Placa = motoCriada.Placa
            };
        }

        public async Task<IEnumerable<MotoDto>> ConsultarMotosAsync(string? placa = null)
        {
            IEnumerable<Moto> motos;

            if (!string.IsNullOrEmpty(placa))
                motos = await _motoRepository.ObterPorPlacaAsync(placa);
            else
                motos = await _motoRepository.ObterTodosAsync();

            return motos.Select(m => new MotoDto
            {
                Identificador = m.Identificador,
                Ano = m.Ano,
                Modelo = m.Modelo,
                Placa = m.Placa
            });
        }

        public async Task<MotoDto?> ConsultarMotoPorIdAsync(string id)
        {
            var moto = await _motoRepository.ObterPorIdAsync(id);
            if (moto == null)
                return null;

            return new MotoDto
            {
                Identificador = moto.Identificador,
                Ano = moto.Ano,
                Modelo = moto.Modelo,
                Placa = moto.Placa
            };
        }

        public async Task<MotoDto?> AtualizarPlacaAsync(string id, string novaPlaca)
        {
            var moto = await _motoRepository.ObterPorIdAsync(id);
            if (moto == null)
                return null;

            // Validar se a nova placa já existe (exceto para a própria moto)
            if (moto.Placa != novaPlaca && await _motoRepository.ExistePlacaAsync(novaPlaca))
                throw new ArgumentException("Já existe uma moto com esta placa");

            moto.Placa = novaPlaca;
            var motoAtualizada = await _motoRepository.AtualizarAsync(moto);

            return new MotoDto
            {
                Identificador = motoAtualizada!.Identificador,
                Ano = motoAtualizada.Ano,
                Modelo = motoAtualizada.Modelo,
                Placa = motoAtualizada.Placa
            };
        }

        public async Task<bool> RemoverMotoAsync(string id)
        {
            var moto = await _motoRepository.ObterPorIdAsync(id);
            if (moto == null)
                return false;

            // Verificar se a moto possui locações
            if (await _motoRepository.PossuiLocacoesAsync(id))
                throw new InvalidOperationException("Não é possível remover uma moto que possui registros de locações");

            return await _motoRepository.RemoverAsync(id);
        }
    }
}

