using AbrigueSe.Dtos;
using AbrigueSe.Models;
using AbrigueSe.Repositories.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AbrigueSe.Controllers
{
    /// <summary>
    /// Gerencia as operações de check-in e check-out de pessoas em abrigos.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CheckInsController : ControllerBase
    {
        private readonly ICheckInRepository _checkInRepository;
        private readonly IMapper _mapper;

        public CheckInsController(ICheckInRepository checkInRepository, IMapper mapper)
        {
            _checkInRepository = checkInRepository;
            _mapper = mapper;
        }

        private void AddLinksToCheckIn(CheckInGetDto checkInDto)
        {
            if (checkInDto == null) return;
            checkInDto.Links.Add(new LinkDto(Url.Link(nameof(GetCheckInById), new { id = checkInDto.IdCheckin }), "self", "GET"));
            // Adicionar outros links relevantes, como para pessoa e abrigo, se houver endpoints para eles.
            // Ex: checkInDto.Links.Add(new LinkDto(Url.Link("GetPessoaById", new { controller = "Pessoas", id = checkInDto.IdPessoa }), "pessoa", "GET"));
            // Ex: checkInDto.Links.Add(new LinkDto(Url.Link("GetAbrigoById", new { controller = "Abrigos", id = checkInDto.IdAbrigo }), "abrigo", "GET"));
        }

        /// <summary>
        /// Realiza o check-in de uma pessoa em um abrigo.
        /// </summary>
        /// <param name="checkInDto">Dados para o check-in.</param>
        /// <response code="201">Check-in realizado com sucesso. Retorna os dados do check-in.</response>
        /// <response code="400">Dados inválidos para o check-in (ex: pessoa já com check-in ativo, capacidade máxima do abrigo atingida).</response>
        /// <response code="404">Abrigo ou Pessoa não encontrado(a).</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpPost]
        [ProducesResponseType(typeof(CheckInGetDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)] // Para Abrigo/Pessoa não encontrado
        [ProducesResponseType(500)]
        public async Task<ActionResult<CheckInGetDto>> CreateCheckIn([FromBody] CheckInDto checkInDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var checkInModel = await _checkInRepository.Create(checkInDto);
                var checkInGetDto = _mapper.Map<CheckInGetDto>(checkInModel);
                AddLinksToCheckIn(checkInGetDto);
                return CreatedAtAction(nameof(GetCheckInById), new { id = checkInGetDto.IdCheckin }, checkInGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("não encontrado")) return NotFound(ex.Message);
                if (ex.Message.Contains("check-in ativo") || ex.Message.Contains("capacidade máxima")) return BadRequest(ex.Message);
                return StatusCode(500, $"Erro interno ao criar o check-in: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtém todos os registros de check-in.
        /// </summary>
        /// <response code="200">Lista de check-ins retornada com sucesso.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpGet("getAll")] // Rota alterada
        [ProducesResponseType(typeof(List<CheckInGetDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<CheckInGetDto>>> GetAll() // Nome do método alterado
        {
            try
            {
                var checkIns = await _checkInRepository.GetAll();
                var checkInsGetDto = _mapper.Map<List<CheckInGetDto>>(checkIns);
                checkInsGetDto.ForEach(AddLinksToCheckIn);
                return Ok(checkInsGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Nenhum registro de check-in encontrado")) return Ok(new List<CheckInGetDto>());
                return StatusCode(500, $"Erro interno ao buscar check-ins: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtém um registro de check-in específico pelo seu ID.
        /// </summary>
        /// <param name="id">ID do check-in.</param>
        /// <response code="200">Check-in retornado com sucesso.</response>
        /// <response code="404">Check-in não encontrado.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CheckInGetDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<CheckInGetDto>> GetCheckInById(int id)
        {
            try
            {
                var checkIn = await _checkInRepository.GetById(id);
                var checkInGetDto = _mapper.Map<CheckInGetDto>(checkIn);
                AddLinksToCheckIn(checkInGetDto);
                return Ok(checkInGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Check-in não encontrado")) return NotFound(ex.Message);
                return StatusCode(500, $"Erro interno ao buscar o check-in: {ex.Message}");
            }
        }

        /// <summary>
        /// Realiza o check-out de uma pessoa de um abrigo.
        /// </summary>
        /// <param name="id">ID do check-in a ser finalizado (check-out).</param>
        /// <response code="200">Check-out realizado com sucesso. Retorna os dados do check-in atualizado.</response>
        /// <response code="404">Check-in não encontrado.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpPut("checkout/{id}")]
        [ProducesResponseType(typeof(CheckInGetDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<CheckInGetDto>> Checkout(int id)
        {
            try
            {
                var checkIn = await _checkInRepository.GetById(id);
                if (checkIn == null)
                {
                    return NotFound($"Check-in com ID {id} não encontrado.");
                }
                // Verifica se o check-in já está finalizado
                if (checkIn.DtSaida.HasValue)
                {
                    return BadRequest("Check-in já finalizado.");
                }
                // Realiza o check-out
                checkIn.DtSaida = DateTime.UtcNow; // Define a data de saída como agora                
                
                var checkInGetDto = _mapper.Map<CheckInGetDto>(checkIn);
                AddLinksToCheckIn(checkInGetDto);
                return Ok(checkInGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Check-in não encontrado")) return NotFound(ex.Message);
                return StatusCode(500, $"Erro interno ao realizar o check-out: {ex.Message}");
            }
        }

        /// <summary>
        /// Exclui um registro de check-in. (Usar com cautela, geralmente check-outs são preferíveis)
        /// </summary>
        /// <param name="id">ID do check-in a ser excluído.</param>
        /// <response code="204">Check-in excluído com sucesso.</response>
        /// <response code="404">Check-in não encontrado.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteCheckIn(int id)
        {
            try
            {
                var sucesso = await _checkInRepository.DeleteById(id);
                if (!sucesso)
                {
                    return NotFound($"Check-in com ID {id} não encontrado.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Check-in não encontrado")) return NotFound(ex.Message);
                return StatusCode(500, $"Erro interno ao excluir o check-in: {ex.Message}");
            }
        }
    }
}