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
    /// Gerencia os tipos de usu�rio do sistema.
    /// </summary>
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
        
        private void AddLinksToTipoUsuario(TipoUsuarioGetDto tipoUsuarioDto)
        {
            if (tipoUsuarioDto == null) return;
            tipoUsuarioDto.Links.Add(new LinkDto(Url.Link(nameof(GetTipoUsuarioById), new { id = tipoUsuarioDto.IdTipoUsuario }), "self", "GET"));
            // Adicionar outros links relevantes, como para atualizar e deletar, se aplic�vel.
             tipoUsuarioDto.Links.Add(new LinkDto(Url.Link(nameof(UpdateTipoUsuario), new { id = tipoUsuarioDto.IdTipoUsuario }), "update_tipousuario", "PUT"));
             tipoUsuarioDto.Links.Add(new LinkDto(Url.Link(nameof(DeleteTipoUsuario), new { id = tipoUsuarioDto.IdTipoUsuario }), "delete_tipousuario", "DELETE"));
        }

        /// <summary>
        /// Cria um novo tipo de usu�rio.
        /// </summary>
        /// <param name="tipoUsuarioDto">Dados para a cria��o do tipo de usu�rio.</param>
        /// <response code="201">Tipo de usu�rio criado com sucesso. Retorna o tipo de usu�rio criado.</response>
        /// <response code="400">Dados inv�lidos (ex: descri��o duplicada).</response>
        /// <response code="500">Erro interno no servidor.</response>
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
                AddLinksToTipoUsuario(tipoUsuarioGetDto);
                return CreatedAtAction(nameof(GetTipoUsuarioById), new { id = tipoUsuarioGetDto.IdTipoUsuario }, tipoUsuarioGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("descri��o")) return BadRequest(ex.Message);
                return StatusCode(500, $"Erro interno ao criar o tipo de usu�rio: {ex.Message}");
            }
        }

        /// <summary>
        /// Obt�m todos os tipos de usu�rio cadastrados.
        /// </summary>
        /// <response code="200">Lista de tipos de usu�rio retornada com sucesso.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpGet("getAll")] // Rota alterada
        [ProducesResponseType(typeof(List<TipoUsuarioGetDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<TipoUsuarioGetDto>>> GetAll() // Nome do m�todo alterado
        {
            try
            {
                var tiposUsuario = await _tipoUsuarioRepository.GetAll();
                var tiposUsuarioGetDto = _mapper.Map<List<TipoUsuarioGetDto>>(tiposUsuario);
                tiposUsuarioGetDto.ForEach(AddLinksToTipoUsuario);
                return Ok(tiposUsuarioGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Nenhum tipo de usu�rio encontrado")) return Ok(new List<TipoUsuarioGetDto>());
                return StatusCode(500, $"Erro interno ao buscar tipos de usu�rio: {ex.Message}");
            }
        }

        /// <summary>
        /// Obt�m um tipo de usu�rio espec�fico pelo seu ID.
        /// </summary>
        /// <param name="id">ID do tipo de usu�rio.</param>
        /// <response code="200">Tipo de usu�rio retornado com sucesso.</response>
        /// <response code="404">Tipo de usu�rio n�o encontrado.</response>
        /// <response code="500">Erro interno no servidor.</response>
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
                AddLinksToTipoUsuario(tipoUsuarioGetDto);
                return Ok(tipoUsuarioGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Tipo de Usu�rio n�o encontrado")) return NotFound(ex.Message);
                return StatusCode(500, $"Erro interno ao buscar o tipo de usu�rio: {ex.Message}");
            }
        }

        /// <summary>
        /// Atualiza um tipo de usu�rio existente.
        /// </summary>
        /// <param name="id">ID do tipo de usu�rio a ser atualizado.</param>
        /// <param name="tipoUsuarioDto">Dados para a atualiza��o.</param>
        /// <response code="200">Tipo de usu�rio atualizado com sucesso. Retorna o tipo de usu�rio atualizado.</response>
        /// <response code="400">Dados inv�lidos (ex: descri��o duplicada).</response>
        /// <response code="404">Tipo de usu�rio n�o encontrado.</response>
        /// <response code="500">Erro interno no servidor.</response>
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
                AddLinksToTipoUsuario(tipoUsuarioGetDto);
                return Ok(tipoUsuarioGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Tipo de Usu�rio n�o encontrado")) return NotFound(ex.Message);
                if (ex.Message.Contains("descri��o")) return BadRequest(ex.Message);
                return StatusCode(500, $"Erro interno ao atualizar o tipo de usu�rio: {ex.Message}");
            }
        }

        /// <summary>
        /// Exclui um tipo de usu�rio.
        /// </summary>
        /// <param name="id">ID do tipo de usu�rio a ser exclu�do.</param>
        /// <response code="204">Tipo de usu�rio exclu�do com sucesso.</response>
        /// <response code="400">N�o � poss�vel excluir o tipo de usu�rio pois ele est� associado a usu�rios.</response>
        /// <response code="404">Tipo de usu�rio n�o encontrado.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteTipoUsuario(int id)
        {
            try
            {
                var sucesso = await _tipoUsuarioRepository.DeleteById(id);
                if (!sucesso)
                {
                    return NotFound($"Tipo de Usu�rio com ID {id} n�o encontrado.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Tipo de Usu�rio n�o encontrado")) return NotFound(ex.Message);
                if (ex.Message.Contains("associado a usu�rios")) return BadRequest(ex.Message);
                return StatusCode(500, $"Erro interno ao excluir o tipo de usu�rio: {ex.Message}");
            }
        }
    }
}