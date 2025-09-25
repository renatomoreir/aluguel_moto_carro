using Microsoft.AspNetCore.Mvc;
using Mottu.Application.DTOs;
using Mottu.Application.Interfaces;

namespace Mottu.API.Controllers
{
    /// <summary>
    /// Controller responsável pelo gerenciamento de entregadores
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class EntregadoresController : ControllerBase
    {
        private readonly IEntregadorService _entregadorService;
        private readonly ILogger<EntregadoresController> _logger;

        public EntregadoresController(IEntregadorService entregadorService, ILogger<EntregadoresController> logger)
        {
            _entregadorService = entregadorService;
            _logger = logger;
        }

        /// <summary>
        /// Cadastrar entregador
        /// </summary>
        /// <param name="criarEntregadorDto">Dados do entregador a ser cadastrado</param>
        /// <returns>Entregador cadastrado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(EntregadorDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EntregadorDto>> CadastrarEntregador([FromBody] CriarEntregadorDto criarEntregadorDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var entregador = await _entregadorService.CriarEntregadorAsync(criarEntregadorDto);
                return CreatedAtAction(nameof(ConsultarEntregadorPorId), new { id = entregador.Identificador }, entregador);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Erro de validação ao cadastrar entregador");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao cadastrar entregador");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Consultar entregador por ID
        /// </summary>
        /// <param name="id">Identificador do entregador</param>
        /// <returns>Entregador encontrado</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EntregadorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EntregadorDto>> ConsultarEntregadorPorId(string id)
        {
            try
            {
                var entregador = await _entregadorService.ConsultarEntregadorPorIdAsync(id);
                if (entregador == null)
                    return NotFound("Entregador não encontrado");

                return Ok(entregador);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao consultar entregador por ID: {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Enviar foto da CNH
        /// </summary>
        /// <param name="id">Identificador do entregador</param>
        /// <param name="arquivo">Arquivo de imagem da CNH</param>
        /// <returns>Resultado da operação</returns>
        [HttpPost("{id}/cnh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> EnviarFotoCnh(string id, IFormFile arquivo)
        {
            try
            {
                if (arquivo == null || arquivo.Length == 0)
                    return BadRequest("Arquivo de imagem é obrigatório");

                // Validar formato do arquivo
                var extensao = Path.GetExtension(arquivo.FileName).ToLowerInvariant();
                if (extensao != ".png" && extensao != ".bmp")
                    return BadRequest("Formato de arquivo inválido. Apenas PNG e BMP são aceitos");

                // Salvar arquivo no storage (implementação simplificada - salvar no disco local)
                var nomeArquivo = $"{id}_{DateTime.UtcNow:yyyyMMddHHmmss}{extensao}";
                var caminhoArquivo = Path.Combine("uploads", "cnh", nomeArquivo);
                
                // Criar diretório se não existir
                Directory.CreateDirectory(Path.GetDirectoryName(caminhoArquivo)!);

                using (var stream = new FileStream(caminhoArquivo, FileMode.Create))
                {
                    await arquivo.CopyToAsync(stream);
                }

                var resultado = await _entregadorService.AtualizarImagemCnhAsync(id, caminhoArquivo);
                if (!resultado)
                    return NotFound("Entregador não encontrado");

                return Ok("Imagem da CNH atualizada com sucesso");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Erro de validação ao enviar foto da CNH para entregador {Id}", id);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao enviar foto da CNH para entregador {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }
    }
}

