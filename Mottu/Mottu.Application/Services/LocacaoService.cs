using Mottu.Application.DTOs;
using Mottu.Application.Interfaces;
using Mottu.Domain.Entities;
using Mottu.Domain.Enums;
using Mottu.Domain.Interfaces;

namespace Mottu.Application.Services
{
    public class LocacaoService : ILocacaoService
    {
        private readonly ILocacaoRepository _locacaoRepository;
        private readonly IEntregadorRepository _entregadorRepository;
        private readonly IMotoRepository _motoRepository;

        public LocacaoService(
            ILocacaoRepository locacaoRepository,
            IEntregadorRepository entregadorRepository,
            IMotoRepository motoRepository)
        {
            _locacaoRepository = locacaoRepository;
            _entregadorRepository = entregadorRepository;
            _motoRepository = motoRepository;
        }

        public async Task<LocacaoDto> CriarLocacaoAsync(CriarLocacaoDto criarLocacaoDto)
        {
            // Validar se o entregador existe
            var entregador = await _entregadorRepository.ObterPorIdAsync(criarLocacaoDto.EntregadorId);
            if (entregador == null)
                throw new ArgumentException("Entregador não encontrado");

            // Validar se o entregador tem CNH categoria A
            if (entregador.TipoCnh != TipoCnh.A && entregador.TipoCnh != TipoCnh.AB)
                throw new InvalidOperationException("Somente entregadores habilitados na categoria A podem efetuar uma locação");

            // Validar se a moto existe
            var moto = await _motoRepository.ObterPorIdAsync(criarLocacaoDto.MotoId);
            if (moto == null)
                throw new ArgumentException("Moto não encontrada");

            // Validar se o entregador já possui locação ativa
            if (await _locacaoRepository.EntregadorPossuiLocacaoAtivaAsync(criarLocacaoDto.EntregadorId))
                throw new InvalidOperationException("Entregador já possui uma locação ativa");

            // Validar se a moto já está alugada
            if (await _locacaoRepository.MotoPossuiLocacaoAtivaAsync(criarLocacaoDto.MotoId))
                throw new InvalidOperationException("Moto já está alugada");

            // Validar plano
            var planosValidos = new[] { 7, 15, 30, 45, 50 };
            if (!planosValidos.Contains(criarLocacaoDto.Plano))
                throw new ArgumentException("Plano inválido. Planos válidos: 7, 15, 30, 45, 50 dias");

            // O início da locação é obrigatoriamente o primeiro dia após a data de criação
            var dataInicio = DateTime.UtcNow.Date.AddDays(1);
            var dataTermino = dataInicio.AddDays(criarLocacaoDto.Plano);

            var locacao = new Locacao
            {
                Identificador = criarLocacaoDto.Identificador,
                EntregadorId = criarLocacaoDto.EntregadorId,
                MotoId = criarLocacaoDto.MotoId,
                DataInicio = dataInicio,
                DataTermino = dataTermino,
                DataPrevisaoTermino = dataTermino,
                Plano = criarLocacaoDto.Plano,
                ValorDiaria = Locacao.ObterValorDiaria(criarLocacaoDto.Plano),
                DataCriacao = DateTime.UtcNow
            };

            var locacaoCriada = await _locacaoRepository.AdicionarAsync(locacao);

            return new LocacaoDto
            {
                Identificador = locacaoCriada.Identificador,
                EntregadorId = locacaoCriada.EntregadorId,
                MotoId = locacaoCriada.MotoId,
                DataInicio = locacaoCriada.DataInicio,
                DataTermino = locacaoCriada.DataTermino,
                DataPrevisaoTermino = locacaoCriada.DataPrevisaoTermino,
                DataDevolucao = locacaoCriada.DataDevolucao,
                Plano = locacaoCriada.Plano,
                ValorDiaria = locacaoCriada.ValorDiaria,
                ValorTotal = locacaoCriada.ValorTotal
            };
        }

        public async Task<LocacaoDto?> ConsultarLocacaoPorIdAsync(string id)
        {
            var locacao = await _locacaoRepository.ObterPorIdAsync(id);
            if (locacao == null)
                return null;

            return new LocacaoDto
            {
                Identificador = locacao.Identificador,
                EntregadorId = locacao.EntregadorId,
                MotoId = locacao.MotoId,
                DataInicio = locacao.DataInicio,
                DataTermino = locacao.DataTermino,
                DataPrevisaoTermino = locacao.DataPrevisaoTermino,
                DataDevolucao = locacao.DataDevolucao,
                Plano = locacao.Plano,
                ValorDiaria = locacao.ValorDiaria,
                ValorTotal = locacao.ValorTotal
            };
        }

        public async Task<CalculoLocacaoDto?> CalcularValorDevolucaoAsync(string id, DateTime dataDevolucao)
        {
            var locacao = await _locacaoRepository.ObterPorIdAsync(id);
            if (locacao == null)
                return null;

            // Validar se a data de devolução é válida
            if (dataDevolucao.Date < locacao.DataInicio.Date)
                throw new ArgumentException("Data de devolução não pode ser anterior à data de início da locação");

            // Atualizar a locação com a data de devolução
            locacao.DataDevolucao = dataDevolucao;
            var valorTotal = locacao.CalcularValorTotal();
            locacao.ValorTotal = valorTotal;

            await _locacaoRepository.AtualizarAsync(locacao);

            var observacoes = "";
            if (dataDevolucao.Date < locacao.DataPrevisaoTermino.Date)
            {
                var percentualMulta = locacao.Plano switch
                {
                    7 => 20,
                    15 => 40,
                    _ => 0
                };
                observacoes = $"Devolução antecipada. Multa de {percentualMulta}% aplicada sobre as diárias não efetivadas.";
            }
            else if (dataDevolucao.Date > locacao.DataPrevisaoTermino.Date)
            {
                var diasAdicionais = (dataDevolucao.Date - locacao.DataPrevisaoTermino.Date).Days;
                observacoes = $"Devolução em atraso. {diasAdicionais} dia(s) adicional(is) cobrado(s) a R$ 50,00 cada.";
            }
            else
            {
                observacoes = "Devolução no prazo previsto.";
            }

            return new CalculoLocacaoDto
            {
                ValorTotal = valorTotal,
                DataDevolucao = dataDevolucao,
                Observacoes = observacoes
            };
        }
    }
}

