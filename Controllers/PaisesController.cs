using AbrigueSe.Dtos;
using AbrigueSe.Models;
using AbrigueSe.Repositories.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<ActionResult<PaisGetDto>> CreatePais([FromBody] PaisDto paisDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var paisModel = await _paisRepository.Create(paisDto);
                var paisGetDto = _mapper.Map<PaisGetDto>(paisModel);

                return CreatedAtAction(nameof(GetPaisById), new { id = paisGetDto.IdPais }, paisGetDto);
            }
            catch (Exception ex)
            {
                // Log a exceção (ex.ToString()) com um logger apropriado
                return StatusCode(500, $"Erro interno ao criar o país: {ex.Message}");
            }
        }

        // GET: api/Paises/getAll
        [HttpGet("getAll")] // Rota alterada
        [ProducesResponseType(typeof(List<PaisGetDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<PaisGetDto>>> GetAll() // Nome do método alterado
        {
            try
            {
                var paises = await _paisRepository.GetAll();
                var paisesGetDto = _mapper.Map<List<PaisGetDto>>(paises);
                return Ok(paisesGetDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao buscar países: {ex.Message}");
            }
        }

        // GET: api/Paises/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PaisGetDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PaisGetDto>> GetPaisById(int id)
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
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao buscar o país: {ex.Message}");
            }
        }

        // PUT: api/Paises/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdatePais(int id, [FromBody] PaisDto paisDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var paisExistente = await _paisRepository.GetById(id);
                if (paisExistente == null)
                {
                    return NotFound($"País com ID {id} não encontrado para atualização.");
                }

                await _paisRepository.Update(paisDto, id);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                 return StatusCode(409, "Conflito de concorrência ao atualizar o país. Tente novamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao atualizar o país: {ex.Message}");
            }
        }

        // DELETE: api/Paises/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeletePais(int id)
        {
            try
            {
                var sucesso = await _paisRepository.Delete(id);
                if (!sucesso)
                {
                    return NotFound($"País com ID {id} não encontrado para exclusão.");
                }
                return NoContent();
            }
            catch (Exception ex) // Capturar exceções específicas de FK se configurado no repositório
            {
                // Exemplo: if (ex.Message.Contains("FOREIGN KEY constraint")) return Conflict("Este país não pode ser excluído pois está associado a estados.");
                return StatusCode(500, $"Erro interno ao excluir o país: {ex.Message}");
            }
        }
    }
}