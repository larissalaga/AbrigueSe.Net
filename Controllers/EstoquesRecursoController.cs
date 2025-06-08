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
    public class EstoquesRecursoController : ControllerBase
    {
        private readonly IEstoqueRecursoRepository _estoqueRecursoRepository;
        private readonly IMapper _mapper;

        public EstoquesRecursoController(IEstoqueRecursoRepository estoqueRecursoRepository, IMapper mapper)
        {
            _estoqueRecursoRepository = estoqueRecursoRepository;
            _mapper = mapper;
        }

        // POST: api/EstoquesRecurso
        [HttpPost]
        [ProducesResponseType(typeof(EstoqueRecursoGetDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)] // Para Abrigo/Recurso não encontrado
        [ProducesResponseType(500)]
        public async Task<ActionResult<EstoqueRecursoGetDto>> CreateEstoqueRecurso([FromBody] EstoqueRecursoDto estoqueDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var estoqueModel = await _estoqueRecursoRepository.Create(estoqueDto);
                var estoqueGetDto = _mapper.Map<EstoqueRecursoGetDto>(estoqueModel);

                return CreatedAtAction(nameof(GetEstoqueRecursoById), new { id = estoqueGetDto.IdEstoque }, estoqueGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("não encontrado")) return NotFound(ex.Message);
                if (ex.Message.Contains("Já existe um estoque")) return BadRequest(ex.Message);
                return StatusCode(500, $"Erro interno ao criar o estoque: {ex.Message}");
            }
        }

        // GET: api/EstoquesRecurso/getAll
        [HttpGet("getAll")] // Rota alterada
        [ProducesResponseType(typeof(List<EstoqueRecursoGetDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<EstoqueRecursoGetDto>>> GetAll() // Nome do método alterado
        {
            try
            {
                var estoques = await _estoqueRecursoRepository.GetAll();
                var estoquesGetDto = _mapper.Map<List<EstoqueRecursoGetDto>>(estoques);
                return Ok(estoquesGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Nenhum estoque encontrado")) return Ok(new List<EstoqueRecursoGetDto>());
                return StatusCode(500, $"Erro interno ao buscar estoques: {ex.Message}");
            }
        }
        
        // GET: api/EstoquesRecurso/abrigo/{idAbrigo}
        [HttpGet("abrigo/{idAbrigo}")]
        [ProducesResponseType(typeof(List<EstoqueRecursoGetDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<EstoqueRecursoGetDto>>> GetEstoquesByAbrigoId(int idAbrigo)
        {
            try
            {
                var estoques = await _estoqueRecursoRepository.GetByAbrigoId(idAbrigo);
                // Se não houver estoques para o abrigo, retorna uma lista vazia com 200 OK.
                var estoquesGetDto = _mapper.Map<List<EstoqueRecursoGetDto>>(estoques);
                return Ok(estoquesGetDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao buscar estoques do abrigo: {ex.Message}");
            }
        }


        // GET: api/EstoquesRecurso/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EstoqueRecursoGetDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<EstoqueRecursoGetDto>> GetEstoqueRecursoById(int id)
        {
            try
            {
                var estoque = await _estoqueRecursoRepository.GetById(id);
                var estoqueGetDto = _mapper.Map<EstoqueRecursoGetDto>(estoque);
                return Ok(estoqueGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Estoque não encontrado")) return NotFound(ex.Message);
                return StatusCode(500, $"Erro interno ao buscar o estoque: {ex.Message}");
            }
        }

        // PUT: api/EstoquesRecurso/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(EstoqueRecursoGetDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<EstoqueRecursoGetDto>> UpdateEstoqueRecurso(int id, [FromBody] EstoqueRecursoDto estoqueDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var estoqueAtualizado = await _estoqueRecursoRepository.UpdateById(id, estoqueDto);
                var estoqueGetDto = _mapper.Map<EstoqueRecursoGetDto>(estoqueAtualizado);
                return Ok(estoqueGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("não encontrado")) return NotFound(ex.Message);
                if (ex.Message.Contains("Já existe um estoque")) return BadRequest(ex.Message);
                return StatusCode(500, $"Erro interno ao atualizar o estoque: {ex.Message}");
            }
        }

        // DELETE: api/EstoquesRecurso/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteEstoqueRecurso(int id)
        {
            try
            {
                var sucesso = await _estoqueRecursoRepository.DeleteById(id);
                if (!sucesso)
                {
                    return NotFound($"Estoque com ID {id} não encontrado.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("não encontrado")) return NotFound(ex.Message);
                return StatusCode(500, $"Erro interno ao excluir o estoque: {ex.Message}");
            }
        }
    }
}