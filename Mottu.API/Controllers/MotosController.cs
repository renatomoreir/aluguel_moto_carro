using Microsoft.AspNetCore.Mvc;
using Mottu.Application.DTOs;
using Mottu.Application.Interfaces;

namespace Mottu.API.Controllers
{
    /// <summary>
    /// Controller responsável pelo gerenciamento de motos
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class MotosController : ControllerBase
    {
        private readonly IMotoService _motoService;
        private readonly ILogger<MotosController> _logger;

        public MotosController(IMotoService motoService, ILogger<MotosController> logger)
        {
            _motoService = motoService;
            _logger = logger;
        }

        /// <summary>
        /// Cadastrar uma nova moto
        /// </summary>
        /// <param name="criarMotoDto">Dados da moto a ser cadastrada</param>
        /// <returns>Dados da moto cadastrada</returns>
        /// <response code="201">Moto cadastrada com sucesso</response>
        /// <response code="400">Dados inválidos ou placa já existe</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(MotoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MotoDto>> CadastrarMoto([FromBody] CriarMotoDto criarMotoDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var moto = await _motoService.CriarMotoAsync(criarMotoDto);
                return CreatedAtAction(nameof(ConsultarMotoPorId), new { id = moto.Identificador }, moto);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Erro de validação ao cadastrar moto");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao cadastrar moto");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Consultar motos cadastradas
        /// </summary>
        /// <param name="placa">Filtro opcional por placa</param>
        /// <returns>Lista de motos</returns>
        /// <response code="200">Lista de motos retornada com sucesso</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MotoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<MotoDto>>> ConsultarMotos([FromQuery] string? placa = null)
        {
            try
            {
                var motos = await _motoService.ConsultarMotosAsync(placa);
                return Ok(motos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao consultar motos");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Consultar moto por identificador
        /// </summary>
        /// <param name="id">Identificador da moto</param>
        /// <returns>Dados da moto</returns>
        /// <response code="200">Moto encontrada</response>
        /// <response code="404">Moto não encontrada</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MotoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MotoDto>> ConsultarMotoPorId(string id)
        {
            try
            {
                var moto = await _motoService.ConsultarMotoPorIdAsync(id);
                if (moto == null)
                    return NotFound("Moto não encontrada");

                return Ok(moto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao consultar moto por ID {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Modificar a placa de uma moto
        /// </summary>
        /// <param name="id">Identificador da moto</param>
        /// <param name="atualizarPlacaDto">Nova placa da moto</param>
        /// <returns>Dados da moto atualizada</returns>
        /// <response code="200">Placa atualizada com sucesso</response>
        /// <response code="400">Dados inválidos ou placa já existe</response>
        /// <response code="404">Moto não encontrada</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPut("{id}/placa")]
        [ProducesResponseType(typeof(MotoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MotoDto>> AtualizarPlaca(string id, [FromBody] AtualizarPlacaMotoDto atualizarPlacaDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var moto = await _motoService.AtualizarPlacaAsync(id, atualizarPlacaDto.Placa);
                if (moto == null)
                    return NotFound("Moto não encontrada");

                return Ok(moto);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Erro de validação ao atualizar placa da moto {Id}", id);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao atualizar placa da moto {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Remover uma moto
        /// </summary>
        /// <param name="id">Identificador da moto</param>
        /// <returns>Resultado da operação</returns>
        /// <response code="204">Moto removida com sucesso</response>
        /// <response code="400">Moto possui locações e não pode ser removida</response>
        /// <response code="404">Moto não encontrada</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> RemoverMoto(string id)
        {
            try
            {
                var removido = await _motoService.RemoverMotoAsync(id);
                if (!removido)
                    return NotFound("Moto não encontrada");

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Tentativa de remover moto {Id} com locações", id);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao remover moto {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }
    }
}

