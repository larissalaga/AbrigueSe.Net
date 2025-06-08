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
    public class PaisesController : ControllerBase
    {
        private readonly IPaisRepository _paisRepository;
        private readonly IMapper _mapper;

        public PaisesController(IPaisRepository paisRepository, IMapper mapper)
        {
            _paisRepository = paisRepository;
            _mapper = mapper;
        }

        // POST: api/Paises
        [HttpPost]
        [ProducesResponseType(typeof(PaisGetDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PaisGetDto>> CreatePais([FromBody] PaisCreateDto paisCreateDto) // Changed to PaisCreateDto
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var paisModel = await _paisRepository.Create(paisCreateDto);
                var paisGetDto = _mapper.Map<PaisGetDto>(paisModel);

                return CreatedAtAction(nameof(GetPaisDetailsById), new { id = paisGetDto.IdPais }, paisGetDto); // Points to GetPaisDetailsById
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("nome")) return BadRequest(ex.Message);
                return StatusCode(500, $"Erro interno ao criar o país: {ex.Message}");
            }
        }

        // GET: api/Paises/getAll
        [HttpGet("getAll")] 
        [ProducesResponseType(typeof(List<PaisGetDto>), 200)] // Returns List<PaisGetDto>
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<PaisGetDto>>> GetAll() 
        {
            try
            {
                var paises = await _paisRepository.GetAll();
                if (paises == null || !paises.Any())
                {
                    return Ok(new List<PaisGetDto>()); // Return empty list if none found
                }
                var paisesGetDto = _mapper.Map<List<PaisGetDto>>(paises);
                return Ok(paisesGetDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao buscar países: {ex.Message}");
            }
        }

        // GET: api/Paises/{id}/details
        [HttpGet("{id}/details")]
        [ProducesResponseType(typeof(PaisGetDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PaisGetDto>> GetPaisDetailsById(int id) // New method for details
        {
            try
            {
                var pais = await _paisRepository.GetById(id);
                if (pais == null)
                {
                    return NotFound($"País com ID {id} não encontrado.");
                }
                var paisGetDto = _mapper.Map<PaisGetDto>(pais);
                return Ok(paisGetDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao buscar detalhes do país: {ex.Message}");
            }
        }

        // PUT: api/Paises/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(PaisGetDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PaisGetDto>> UpdatePais(int id, [FromBody] PaisUpdateDto paisUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedPais = await _paisRepository.Update(paisUpdateDto, id);
                var paisGetDto = _mapper.Map<PaisGetDto>(updatedPais);
                return Ok(paisGetDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("nome")) return BadRequest(ex.Message);
                return StatusCode(500, $"Erro interno ao atualizar o país: {ex.Message}");
            }
        }

        // DELETE: api/Paises/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(204)] // NoContent
        [ProducesResponseType(404)]
        [ProducesResponseType(400)] // For business rule violations like dependent entities
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeletePais(int id)
        {
            try
            {
                var success = await _paisRepository.Delete(id);
                if (!success)
                {
                    // This case might not be reached if Delete throws KeyNotFoundException for not found.
                    // However, it's good practice if the repository could return false for other reasons.
                    return NotFound($"País com ID {id} não encontrado para exclusão.");
                }
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex) // Catching specific exception for business rule violation
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao excluir o país: {ex.Message}");
            }
        }
    }
}