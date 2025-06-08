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
    public class CidadesController : ControllerBase
    {
        private readonly ICidadeRepository _cidadeRepository;
        private readonly IMapper _mapper;

        public CidadesController(ICidadeRepository cidadeRepository, IMapper mapper)
        {
            _cidadeRepository = cidadeRepository;
            _mapper = mapper;
        }

        // POST: api/Cidades
        [HttpPost]
        [ProducesResponseType(typeof(CidadeGetDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<CidadeGetDto>> CreateCidade([FromBody] CidadeDto cidadeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var cidadeModel = await _cidadeRepository.Create(cidadeDto);
                var cidadeGetDto = _mapper.Map<CidadeGetDto>(cidadeModel);

                return CreatedAtAction(nameof(GetCidadeById), new { id = cidadeGetDto.IdCidade }, cidadeGetDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao criar a cidade: {ex.Message}");
            }
        }

        // GET: api/Cidades/getAll
        [HttpGet("getAll")] // Rota alterada
        [ProducesResponseType(typeof(List<CidadeGetDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<CidadeGetDto>>> GetAll() // Nome do método alterado
        {
            try
            {
                var cidades = await _cidadeRepository.GetAll();
                var cidadesGetDto = _mapper.Map<List<CidadeGetDto>>(cidades);
                return Ok(cidadesGetDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao buscar cidades: {ex.Message}");
            }
        }

        // GET: api/Cidades/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CidadeGetDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<CidadeGetDto>> GetCidadeById(int id)
        {
            try
            {
                var cidade = await _cidadeRepository.GetById(id);
                if (cidade == null)
                {
                    return NotFound($"Cidade com ID {id} não encontrada.");
                }
                var cidadeGetDto = _mapper.Map<CidadeGetDto>(cidade);
                return Ok(cidadeGetDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao buscar a cidade: {ex.Message}");
            }
        }

        // PUT: api/Cidades/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateCidade(int id, [FromBody] CidadeDto cidadeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var cidadeExistente = await _cidadeRepository.GetById(id);
                if (cidadeExistente == null)
                {
                    return NotFound($"Cidade com ID {id} não encontrada para atualização.");
                }

                var cidade = await _cidadeRepository.Update(cidadeDto, id);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                 return StatusCode(409, "Conflito de concorrência ao atualizar a cidade. Tente novamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao atualizar a cidade: {ex.Message}");
            }
        }

        // DELETE: api/Cidades/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteCidade(int id)
        {
            try
            {
                var sucesso = await _cidadeRepository.Delete(id);
                if (!sucesso)
                {
                    return NotFound($"Cidade com ID {id} não encontrada para exclusão.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao excluir a cidade: {ex.Message}");
            }
        }
    }
}