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
    public class EstadosController : ControllerBase
    {
        private readonly IEstadoRepository _estadoRepository;
        private readonly IMapper _mapper;

        public EstadosController(IEstadoRepository estadoRepository, IMapper mapper)
        {
            _estadoRepository = estadoRepository;
            _mapper = mapper;
        }

        // POST: api/Estados
        [HttpPost]
        [ProducesResponseType(typeof(EstadoGetDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<EstadoGetDto>> CreateEstado([FromBody] EstadoDto estadoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {                
                var estadoModel = await _estadoRepository.Create(estadoDto); // Assume que Create atualiza o ID no estadoModel
                
                var estadoGetDto = _mapper.Map<EstadoGetDto>(estadoModel); // Mapeia após o Create para obter o ID do país

                return CreatedAtAction(nameof(GetEstadoById), new { id = estadoGetDto.IdEstado }, estadoGetDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao criar o estado: {ex.Message}");
            }
        }

        // GET: api/Estados/getAll
        [HttpGet("getAll")] // Rota alterada
        [ProducesResponseType(typeof(List<EstadoGetDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<EstadoGetDto>>> GetAll() // Nome do método alterado
        {
            try
            {
                var estados = await _estadoRepository.GetAll();
                var estadosGetDto = _mapper.Map<List<EstadoGetDto>>(estados);
                return Ok(estadosGetDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao buscar estados: {ex.Message}");
            }
        }

        // GET: api/Estados/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EstadoGetDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<EstadoGetDto>> GetEstadoById(int id)
        {
            try
            {
                var estado = await _estadoRepository.GetById(id);
                if (estado == null)
                {
                    return NotFound($"Estado com ID {id} não encontrado.");
                }
                var estadoGetDto = _mapper.Map<EstadoGetDto>(estado);
                return Ok(estadoGetDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao buscar o estado: {ex.Message}");
            }
        }

        // PUT: api/Estados/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateEstado(int id, [FromBody] EstadoDto estadoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var estadoExistente = await _estadoRepository.GetById(id);
                if (estadoExistente == null)
                {
                    return NotFound($"Estado com ID {id} não encontrado para atualização.");
                }

                await _estadoRepository.Update(estadoDto, id);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                 return StatusCode(409, "Conflito de concorrência ao atualizar o estado. Tente novamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao atualizar o estado: {ex.Message}");
            }
        }

        // DELETE: api/Estados/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteEstado(int id)
        {
            try
            {
                var sucesso = await _estadoRepository.Delete(id);
                if (!sucesso)
                {
                    return NotFound($"Estado com ID {id} não encontrado para exclusão.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao excluir o estado: {ex.Message}");
            }
        }
    }
}