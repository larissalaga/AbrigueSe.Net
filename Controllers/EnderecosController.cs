using AbrigueSe.Dtos;
using AbrigueSe.Models;
using AbrigueSe.Repositories.Implementations;
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
    public class EnderecosController : ControllerBase
    {
        private readonly IEnderecoRepository _enderecoRepository;
        private readonly IMapper _mapper;

        public EnderecosController(IEnderecoRepository enderecoRepository, IMapper mapper)
        {
            _enderecoRepository = enderecoRepository;
            _mapper = mapper;
        }

        // POST: api/Enderecos
        [HttpPost]
        [ProducesResponseType(typeof(EnderecoGetDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<EnderecoGetDto>> CreateEndereco([FromBody] EnderecoDto enderecoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {   
                var enderecoModel = await _enderecoRepository.Create(enderecoDto);
                var enderecoGetDto = _mapper.Map<EnderecoGetDto>(enderecoModel);

                return CreatedAtAction(nameof(GetEnderecoById), new { id = enderecoGetDto.IdEndereco }, enderecoGetDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao criar o endereço: {ex.Message}");
            }
        }

        // GET: api/Enderecos/getAll
        [HttpGet("getAll")] // Rota alterada
        [ProducesResponseType(typeof(List<EnderecoGetDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<EnderecoGetDto>>> GetAll() // Nome do método alterado
        {
            try
            {
                var enderecos = await _enderecoRepository.GetAll();
                var enderecosGetDto = _mapper.Map<List<EnderecoGetDto>>(enderecos);
                return Ok(enderecosGetDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao buscar endereços: {ex.Message}");
            }
        }

        // GET: api/Enderecos/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EnderecoGetDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<EnderecoGetDto>> GetEnderecoById(int id)
        {
            try
            {
                var endereco = await _enderecoRepository.GetById(id);
                if (endereco == null)
                {
                    return NotFound($"Endereço com ID {id} não encontrado.");
                }
                var enderecoGetDto = _mapper.Map<EnderecoGetDto>(endereco);
                return Ok(enderecoGetDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao buscar o endereço: {ex.Message}");
            }
        }

        // PUT: api/Enderecos/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateEndereco(int id, [FromBody] EnderecoDto enderecoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var enderecoExistente = await _enderecoRepository.GetById(id);
                if (enderecoExistente == null)
                {
                    return NotFound($"Endereço com ID {id} não encontrado para atualização.");
                }

                var enderecoAtualizado = await _enderecoRepository.Update(enderecoDto, id);
                var enderecoGetDto = _mapper.Map<EnderecoGetDto>(enderecoAtualizado);
                return Ok(enderecoGetDto);
            }
            catch (DbUpdateConcurrencyException)
            {
                 return StatusCode(409, "Conflito de concorrência ao atualizar o endereço. Tente novamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao atualizar o endereço: {ex.Message}");
            }
        }

        // DELETE: api/Enderecos/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteEndereco(int id)
        {
            try
            {
                var sucesso = await _enderecoRepository.Delete(id);
                if (!sucesso)
                {
                    return NotFound($"Endereço com ID {id} não encontrado para exclusão.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao excluir o endereço: {ex.Message}");
            }
        }
    }
}