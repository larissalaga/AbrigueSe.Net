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
    public class RecursosController : ControllerBase
    {
        private readonly IRecursoRepository _recursoRepository;
        private readonly IMapper _mapper;

        public RecursosController(IRecursoRepository recursoRepository, IMapper mapper)
        {
            _recursoRepository = recursoRepository;
            _mapper = mapper;
        }

        // POST: api/Recursos
        [HttpPost]
        [ProducesResponseType(typeof(RecursoGetDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<RecursoGetDto>> CreateRecurso([FromBody] RecursoDto recursoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var recursoModel = await _recursoRepository.Create(recursoDto);
                var recursoGetDto = _mapper.Map<RecursoGetDto>(recursoModel);

                return CreatedAtAction(nameof(GetRecursoById), new { id = recursoGetDto.IdRecurso }, recursoGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("descrição")) return BadRequest(ex.Message);
                return StatusCode(500, $"Erro interno ao criar o recurso: {ex.Message}");
            }
        }

        // GET: api/Recursos/getAll
        [HttpGet("getAll")] // Rota alterada
        [ProducesResponseType(typeof(List<RecursoGetDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<RecursoGetDto>>> GetAll() // Nome do método alterado
        {
            try
            {
                var recursos = await _recursoRepository.GetAll();
                var recursosGetDto = _mapper.Map<List<RecursoGetDto>>(recursos);
                return Ok(recursosGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Nenhum recurso encontrado")) return Ok(new List<RecursoGetDto>());
                return StatusCode(500, $"Erro interno ao buscar recursos: {ex.Message}");
            }
        }

        // GET: api/Recursos/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RecursoGetDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<RecursoGetDto>> GetRecursoById(int id)
        {
            try
            {
                var recurso = await _recursoRepository.GetById(id);
                var recursoGetDto = _mapper.Map<RecursoGetDto>(recurso);
                return Ok(recursoGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Recurso não encontrado")) return NotFound(ex.Message);
                return StatusCode(500, $"Erro interno ao buscar o recurso: {ex.Message}");
            }
        }

        // PUT: api/Recursos/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(RecursoGetDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<RecursoGetDto>> UpdateRecurso(int id, [FromBody] RecursoDto recursoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var recursoAtualizado = await _recursoRepository.UpdateById(id, recursoDto);
                var recursoGetDto = _mapper.Map<RecursoGetDto>(recursoAtualizado);
                return Ok(recursoGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Recurso não encontrado")) return NotFound(ex.Message);
                if (ex.Message.Contains("descrição")) return BadRequest(ex.Message);
                return StatusCode(500, $"Erro interno ao atualizar o recurso: {ex.Message}");
            }
        }

        // DELETE: api/Recursos/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteRecurso(int id)
        {
            try
            {
                var sucesso = await _recursoRepository.DeleteById(id);
                 if (!sucesso) // O repo já lança exceção se não encontrar ou se estiver em uso
                {
                    return NotFound($"Recurso com ID {id} não encontrado.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Recurso não encontrado")) return NotFound(ex.Message);
                if (ex.Message.Contains("associado a um estoque")) return BadRequest(ex.Message);
                return StatusCode(500, $"Erro interno ao excluir o recurso: {ex.Message}");
            }
        }
    }
}