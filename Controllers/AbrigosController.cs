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
    public class AbrigosController : ControllerBase
    {
        private readonly IAbrigoRepository _abrigoRepository;
        private readonly IMapper _mapper;

        public AbrigosController(IAbrigoRepository abrigoRepository, IMapper mapper)
        {
            _abrigoRepository = abrigoRepository;
            _mapper = mapper;
        }

        // POST: api/Abrigos
        [HttpPost]
        [ProducesResponseType(typeof(AbrigoGetDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<AbrigoGetDto>> CreateAbrigo([FromBody] AbrigoCreateDto abrigoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var abrigoModel = await _abrigoRepository.Create(abrigoDto);
                var abrigoGetDto = _mapper.Map<AbrigoGetDto>(abrigoModel);

                return CreatedAtAction(nameof(GetAbrigoById), new { id = abrigoGetDto.IdAbrigo }, abrigoGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("nome")) return BadRequest(ex.Message); // Erro de nome duplicado
                return StatusCode(500, $"Erro interno ao criar o abrigo: {ex.Message}");
            }
        }

        // GET: api/Abrigos/getAll
        [HttpGet("getAll")]
        [ProducesResponseType(typeof(List<Abrigo>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<Abrigo>>> GetAll()
        {
            try
            {
                var abrigos = await _abrigoRepository.GetAll();                
                return Ok(abrigos);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Nenhum abrigo encontrado")) return Ok(new List<Abrigo>());
                return StatusCode(500, $"Erro interno ao buscar abrigos: {ex.Message}");
            }
        }

        // GET: api/Abrigos/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Abrigo), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Abrigo>> GetAbrigoById(int id)
        {
            try
            {
                var abrigo = await _abrigoRepository.GetById(id);                
                return Ok(abrigo);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Abrigo não encontrado")) return NotFound(ex.Message);
                return StatusCode(500, $"Erro interno ao buscar o abrigo: {ex.Message}");
            }
        }

        // GET: api/Abrigos/{id}/details
        [HttpGet("{id}/details")]
        [ProducesResponseType(typeof(AbrigoGetDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<AbrigoGetDto>> GetAbrigoDetailsById(int id)
        {
            try
            {
                var abrigoGetDto = await _abrigoRepository.GetDetailsByIdAsync(id);                
                return Ok(abrigoGetDto);
            }
            catch (KeyNotFoundException knfex)
            {
                return NotFound(knfex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao buscar detalhes do abrigo: {ex.Message}");
            }
        }

        // PUT: api/Abrigos/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(AbrigoGetDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<AbrigoGetDto>> UpdateAbrigo(int id, [FromBody] AbrigoCreateDto abrigoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var abrigoAtualizado = await _abrigoRepository.UpdateById(id, abrigoDto);                
                return Ok(abrigoAtualizado);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Abrigo não encontrado")) return NotFound(ex.Message);
                if (ex.Message.Contains("nome")) return BadRequest(ex.Message); // Erro de nome duplicado
                return StatusCode(500, $"Erro interno ao atualizar o abrigo: {ex.Message}");
            }
        }

        // DELETE: api/Abrigos/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)] // Para FK constraints
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteAbrigo(int id)
        {
            try
            {
                // Adicionar verificação no repositório se o abrigo tem check-ins ou estoque antes de excluir
                var sucesso = await _abrigoRepository.DeleteById(id);
                if (!sucesso) // O repo já lança exceção se não encontrar
                {
                     return NotFound($"Abrigo com ID {id} não encontrado.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Abrigo não encontrado")) return NotFound(ex.Message);
                // Adicionar tratamento para FKs se o repositório lançar exceções específicas
                // Ex: if (ex.Message.Contains("check-ins ativos")) return BadRequest("Este abrigo possui check-ins ativos e não pode ser excluído.");
                return StatusCode(500, $"Erro interno ao excluir o abrigo: {ex.Message}");
            }
        }
    }
}