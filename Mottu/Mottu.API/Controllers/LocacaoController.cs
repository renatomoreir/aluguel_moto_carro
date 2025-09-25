using Microsoft.AspNetCore.Mvc;
using Mottu.Application.DTOs;
using Mottu.Application.Interfaces;

namespace Mottu.API.Controllers
{
    /// <summary>
    /// Controller responsável pelo gerenciamento de locações de motos
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class LocacaoController : ControllerBase
    {
        private readonly ILocacaoService _locacaoService;
        private readonly ILogger<LocacaoController> _logger;

        public LocacaoController(ILocacaoService locacaoService, ILogger<LocacaoController> logger)
        {
            _locacaoService = locacaoService;
            _logger = logger;
        }

        /// <summary>
        /// Alugar uma moto
        /// </summary>
        /// <param name="criarLocacaoDto">Dados da locação a ser criada</param>
        /// <returns>Locação criada</returns>
        [HttpPost]
        [ProducesResponseType(typeof(LocacaoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LocacaoDto>> AlugarMoto([FromBody] CriarLocacaoDto criarLocacaoDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var locacao = await _locacaoService.CriarLocacaoAsync(criarLocacaoDto);
                return CreatedAtAction(nameof(ConsultarLocacaoPorId), new { id = locacao.Identificador }, locacao);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Erro de validação ao criar locação");
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Operação inválida ao criar locação");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao criar locação");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Consultar locação por ID
        /// </summary>
        /// <param name="id">Identificador da locação</param>
        /// <returns>Locação encontrada</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(LocacaoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LocacaoDto>> ConsultarLocacaoPorId(string id)
        {
            try
            {
                var locacao = await _locacaoService.ConsultarLocacaoPorIdAsync(id);
                if (locacao == null)
                    return NotFound("Locação não encontrada");

                return Ok(locacao);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao consultar locação por ID: {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Informar data de devolução e calcular valor
        /// </summary>
        /// <param name="id">Identificador da locação</param>
        /// <param name="devolucaoDto">Data de devolução</param>
        /// <returns>Cálculo do valor total da locação</returns>
        [HttpPut("{id}/devolucao")]
        [ProducesResponseType(typeof(CalculoLocacaoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CalculoLocacaoDto>> InformarDevolucao(string id, [FromBody] DevolucaoDto devolucaoDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var calculo = await _locacaoService.CalcularValorDevolucaoAsync(id, devolucaoDto.DataDevolucao);
                if (calculo == null)
                    return NotFound("Locação não encontrada");

                return Ok(calculo);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Erro de validação ao informar devolução da locação {Id}", id);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao informar devolução da locação {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }
    }
}

