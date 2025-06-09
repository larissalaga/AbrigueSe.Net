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
    /// Gerencia os tipos de usuário do sistema.
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
            // Adicionar outros links relevantes, como para atualizar e deletar, se aplicável.
             tipoUsuarioDto.Links.Add(new LinkDto(Url.Link(nameof(UpdateTipoUsuario), new { id = tipoUsuarioDto.IdTipoUsuario }), "update_tipousuario", "PUT"));
             tipoUsuarioDto.Links.Add(new LinkDto(Url.Link(nameof(DeleteTipoUsuario), new { id = tipoUsuarioDto.IdTipoUsuario }), "delete_tipousuario", "DELETE"));
        }

        /// <summary>
        /// Cria um novo tipo de usuário.
        /// </summary>
        /// <param name="tipoUsuarioDto">Dados para a criação do tipo de usuário.</param>
        /// <response code="201">Tipo de usuário criado com sucesso. Retorna o tipo de usuário criado.</response>
        /// <response code="400">Dados inválidos (ex: descrição duplicada).</response>
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
                if (ex.Message.Contains("descrição")) return BadRequest(ex.Message);
                return StatusCode(500, $"Erro interno ao criar o tipo de usuário: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtém todos os tipos de usuário cadastrados.
        /// </summary>
        /// <response code="200">Lista de tipos de usuário retornada com sucesso.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpGet("getAll")] // Rota alterada
        [ProducesResponseType(typeof(List<TipoUsuarioGetDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<TipoUsuarioGetDto>>> GetAll() // Nome do método alterado
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
                if (ex.Message.Contains("Nenhum tipo de usuário encontrado")) return Ok(new List<TipoUsuarioGetDto>());
                return StatusCode(500, $"Erro interno ao buscar tipos de usuário: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtém um tipo de usuário específico pelo seu ID.
        /// </summary>
        /// <param name="id">ID do tipo de usuário.</param>
        /// <response code="200">Tipo de usuário retornado com sucesso.</response>
        /// <response code="404">Tipo de usuário não encontrado.</response>
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
                if (ex.Message.Contains("Tipo de Usuário não encontrado")) return NotFound(ex.Message);
                return StatusCode(500, $"Erro interno ao buscar o tipo de usuário: {ex.Message}");
            }
        }

        /// <summary>
        /// Atualiza um tipo de usuário existente.
        /// </summary>
        /// <param name="id">ID do tipo de usuário a ser atualizado.</param>
        /// <param name="tipoUsuarioDto">Dados para a atualização.</param>
        /// <response code="200">Tipo de usuário atualizado com sucesso. Retorna o tipo de usuário atualizado.</response>
        /// <response code="400">Dados inválidos (ex: descrição duplicada).</response>
        /// <response code="404">Tipo de usuário não encontrado.</response>
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
                if (ex.Message.Contains("Tipo de Usuário não encontrado")) return NotFound(ex.Message);
                if (ex.Message.Contains("descrição")) return BadRequest(ex.Message);
                return StatusCode(500, $"Erro interno ao atualizar o tipo de usuário: {ex.Message}");
            }
        }

        /// <summary>
        /// Exclui um tipo de usuário.
        /// </summary>
        /// <param name="id">ID do tipo de usuário a ser excluído.</param>
        /// <response code="204">Tipo de usuário excluído com sucesso.</response>
        /// <response code="400">Não é possível excluir o tipo de usuário pois ele está associado a usuários.</response>
        /// <response code="404">Tipo de usuário não encontrado.</response>
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
                    return NotFound($"Tipo de Usuário com ID {id} não encontrado.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Tipo de Usuário não encontrado")) return NotFound(ex.Message);
                if (ex.Message.Contains("associado a usuários")) return BadRequest(ex.Message);
                return StatusCode(500, $"Erro interno ao excluir o tipo de usuário: {ex.Message}");
            }
        }
    }
}