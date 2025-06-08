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

        // POST: api/CheckIns
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

                return CreatedAtAction(nameof(GetCheckInById), new { id = checkInGetDto.IdCheckin }, checkInGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("não encontrado")) return NotFound(ex.Message);
                if (ex.Message.Contains("check-in ativo") || ex.Message.Contains("capacidade máxima")) return BadRequest(ex.Message);
                return StatusCode(500, $"Erro interno ao criar o check-in: {ex.Message}");
            }
        }

        // GET: api/CheckIns/getAll
        [HttpGet("getAll")] // Rota alterada
        [ProducesResponseType(typeof(List<CheckInGetDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<CheckInGetDto>>> GetAll() // Nome do método alterado
        {
            try
            {
                var checkIns = await _checkInRepository.GetAll();
                var checkInsGetDto = _mapper.Map<List<CheckInGetDto>>(checkIns);
                return Ok(checkInsGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Nenhum registro de check-in encontrado")) return Ok(new List<CheckInGetDto>());
                return StatusCode(500, $"Erro interno ao buscar check-ins: {ex.Message}");
            }
        }

        // GET: api/CheckIns/{id}
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
                return Ok(checkInGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("não encontrado")) return NotFound(ex.Message);
                return StatusCode(500, $"Erro interno ao buscar o check-in: {ex.Message}");
            }
        }
        
        // GET: api/CheckIns/pessoa/{idPessoa}/ativo
        [HttpGet("pessoa/{idPessoa}/ativo")]
        [ProducesResponseType(typeof(CheckInGetDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<CheckInGetDto>> GetActiveCheckInByPessoa(int idPessoa)
        {
            try
            {
                var checkIn = await _checkInRepository.GetActiveCheckInByPessoaId(idPessoa);
                if (checkIn == null)
                {
                    return NotFound($"Nenhum check-in ativo encontrado para a pessoa com ID {idPessoa}.");
                }
                var checkInGetDto = _mapper.Map<CheckInGetDto>(checkIn);
                return Ok(checkInGetDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao buscar o check-in ativo: {ex.Message}");
            }
        }


        // PUT: api/CheckIns/{id} (Principalmente para registrar DtSaida)
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(CheckInGetDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<CheckInGetDto>> UpdateCheckIn(int id, [FromBody] CheckInDto checkInDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var checkInAtualizado = await _checkInRepository.UpdateById(id, checkInDto);
                var checkInGetDto = _mapper.Map<CheckInGetDto>(checkInAtualizado);
                return Ok(checkInGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("não encontrado")) return NotFound(ex.Message);
                if (ex.Message.Contains("capacidade máxima")) return BadRequest(ex.Message);
                return StatusCode(500, $"Erro interno ao atualizar o check-in: {ex.Message}");
            }
        }

        // DELETE: api/CheckIns/{id}
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
                 if (ex.Message.Contains("não encontrado")) return NotFound(ex.Message);
                return StatusCode(500, $"Erro interno ao excluir o check-in: {ex.Message}");
            }
        }
    }
}