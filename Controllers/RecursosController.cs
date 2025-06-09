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
    /// Gerencia as operações relacionadas a recursos (suprimentos).
    /// </summary>
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

        private void AddLinksToRecurso(RecursoGetDto recursoDto)
        {
            if (recursoDto == null) return;

            recursoDto.Links.Add(new LinkDto(Url.Link(nameof(GetRecursoById), new { id = recursoDto.IdRecurso }), "self", "GET"));
            recursoDto.Links.Add(new LinkDto(Url.Link(nameof(UpdateRecurso), new { id = recursoDto.IdRecurso }), "update_recurso", "PUT"));
            recursoDto.Links.Add(new LinkDto(Url.Link(nameof(DeleteRecurso), new { id = recursoDto.IdRecurso }), "delete_recurso", "DELETE"));
        }

        // POST: api/Recursos
        /// <summary>
        /// Cria um novo tipo de recurso.
        /// </summary>
        /// <param name="recursoDto">Dados para a criação do recurso.</param>
        /// <response code="201">Recurso criado com sucesso. Retorna o recurso criado.</response>
        /// <response code="400">Dados inválidos para a criação do recurso (ex: descrição duplicada).</response>
        /// <response code="500">Erro interno no servidor.</response>
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
                AddLinksToRecurso(recursoGetDto);

                return CreatedAtAction(nameof(GetRecursoById), new { id = recursoGetDto.IdRecurso }, recursoGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("descrição")) return BadRequest(ex.Message);
                return StatusCode(500, $"Erro interno ao criar o recurso: {ex.Message}");
            }
        }

        // GET: api/Recursos/getAll
        /// <summary>
        /// Obtém todos os tipos de recursos cadastrados.
        /// </summary>
        /// <response code="200">Lista de recursos retornada com sucesso.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpGet("getAll")] // Rota alterada
        [ProducesResponseType(typeof(List<RecursoGetDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<RecursoGetDto>>> GetAll() // Nome do método alterado
        {
            try
            {
                var recursos = await _recursoRepository.GetAll();
                var recursosGetDto = _mapper.Map<List<RecursoGetDto>>(recursos);
                recursosGetDto.ForEach(AddLinksToRecurso);
                return Ok(recursosGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Nenhum recurso encontrado")) return Ok(new List<RecursoGetDto>());
                return StatusCode(500, $"Erro interno ao buscar recursos: {ex.Message}");
            }
        }

        // GET: api/Recursos/{id}
        /// <summary>
        /// Obtém um tipo de recurso específico pelo seu ID.
        /// </summary>
        /// <param name="id">ID do recurso a ser obtido.</param>
        /// <response code="200">Recurso retornado com sucesso.</response>
        /// <response code="404">Recurso não encontrado.</response>
        /// <response code="500">Erro interno no servidor.</response>
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
                AddLinksToRecurso(recursoGetDto);
                return Ok(recursoGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Recurso não encontrado")) return NotFound(ex.Message);
                return StatusCode(500, $"Erro interno ao buscar o recurso: {ex.Message}");
            }
        }

        // PUT: api/Recursos/{id}
        /// <summary>
        /// Atualiza um tipo de recurso existente.
        /// </summary>
        /// <param name="id">ID do recurso a ser atualizado.</param>
        /// <param name="recursoDto">Dados para a atualização do recurso.</param>
        /// <response code="200">Recurso atualizado com sucesso. Retorna o recurso atualizado.</response>
        /// <response code="400">Dados inválidos para a atualização (ex: descrição duplicada).</response>
        /// <response code="404">Recurso não encontrado.</response>
        /// <response code="500">Erro interno no servidor.</response>
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
                AddLinksToRecurso(recursoGetDto);
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