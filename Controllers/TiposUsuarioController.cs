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
    public class TiposUsuarioController : ControllerBase
    {
        private readonly ITipoUsuarioRepository _tipoUsuarioRepository;
        private readonly IMapper _mapper;

        public TiposUsuarioController(ITipoUsuarioRepository tipoUsuarioRepository, IMapper mapper)
        {
            _tipoUsuarioRepository = tipoUsuarioRepository;
            _mapper = mapper;
        }

        // POST: api/TiposUsuario
        [HttpPost]
        [ProducesResponseType(typeof(TipoUsuarioGetDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<TipoUsuarioGetDto>> CreateTipoUsuario([FromBody] TipoUsuarioDto tipoUsuarioDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var tipoUsuarioModel = await _tipoUsuarioRepository.Create(tipoUsuarioDto);
                var tipoUsuarioGetDto = _mapper.Map<TipoUsuarioGetDto>(tipoUsuarioModel);

                return CreatedAtAction(nameof(GetTipoUsuarioById), new { id = tipoUsuarioGetDto.IdTipoUsuario }, tipoUsuarioGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("descri��o")) return BadRequest(ex.Message);
                return StatusCode(500, $"Erro interno ao criar o tipo de usu�rio: {ex.Message}");
            }
        }

        // GET: api/TiposUsuario/getAll
        [HttpGet("getAll")] // Rota alterada
        [ProducesResponseType(typeof(List<TipoUsuarioGetDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<TipoUsuarioGetDto>>> GetAll() // Nome do m�todo alterado
        {
            try
            {
                var tiposUsuario = await _tipoUsuarioRepository.GetAll();
                var tiposUsuarioGetDto = _mapper.Map<List<TipoUsuarioGetDto>>(tiposUsuario);
                return Ok(tiposUsuarioGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Nenhum tipo de usu�rio encontrado")) return Ok(new List<TipoUsuarioGetDto>());
                return StatusCode(500, $"Erro interno ao buscar tipos de usu�rio: {ex.Message}");
            }
        }

        // GET: api/TiposUsuario/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TipoUsuarioGetDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<TipoUsuarioGetDto>> GetTipoUsuarioById(int id)
        {
            try
            {
                var tipoUsuario = await _tipoUsuarioRepository.GetById(id);
                var tipoUsuarioGetDto = _mapper.Map<TipoUsuarioGetDto>(tipoUsuario);
                return Ok(tipoUsuarioGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Tipo de usu�rio n�o encontrado")) return NotFound(ex.Message);
                return StatusCode(500, $"Erro interno ao buscar o tipo de usu�rio: {ex.Message}");
            }
        }

        // PUT: api/TiposUsuario/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TipoUsuarioGetDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<TipoUsuarioGetDto>> UpdateTipoUsuario(int id, [FromBody] TipoUsuarioDto tipoUsuarioDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var tipoUsuarioAtualizado = await _tipoUsuarioRepository.UpdateById(id, tipoUsuarioDto);
                var tipoUsuarioGetDto = _mapper.Map<TipoUsuarioGetDto>(tipoUsuarioAtualizado);
                return Ok(tipoUsuarioGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Tipo de usu�rio n�o encontrado")) return NotFound(ex.Message);
                if (ex.Message.Contains("descri��o")) return BadRequest(ex.Message);
                return StatusCode(500, $"Erro interno ao atualizar o tipo de usu�rio: {ex.Message}");
            }
        }

        // DELETE: api/TiposUsuario/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteTipoUsuario(int id)
        {
            try
            {
                var sucesso = await _tipoUsuarioRepository.DeleteById(id);
                if (!sucesso) // O repo j� lan�a exce��o se n�o encontrar ou se estiver em uso
                {
                    return NotFound($"Tipo de usu�rio com ID {id} n�o encontrado.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Tipo de usu�rio n�o encontrado")) return NotFound(ex.Message);
                if (ex.Message.Contains("associado a um ou mais usu�rios")) return BadRequest(ex.Message);
                return StatusCode(500, $"Erro interno ao excluir o tipo de usu�rio: {ex.Message}");
            }
        }
    }
}