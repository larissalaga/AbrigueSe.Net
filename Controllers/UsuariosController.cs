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
    /// Gerencia as opera��es relacionadas a usu�rios.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMapper _mapper;

        public UsuariosController(IUsuarioRepository usuarioRepository, IMapper mapper)
        {
            _usuarioRepository = usuarioRepository;
            _mapper = mapper;
        }

        private void AddLinksToUsuario(UsuarioGetDto usuarioDto)
        {
            if (usuarioDto == null) return;

            usuarioDto.Links.Add(new LinkDto(Url.Link(nameof(GetUsuarioById), new { id = usuarioDto.IdUsuario }), "self", "GET"));
            usuarioDto.Links.Add(new LinkDto(Url.Link(nameof(UpdateUsuario), new { id = usuarioDto.IdUsuario }), "update_usuario", "PUT"));
            usuarioDto.Links.Add(new LinkDto(Url.Link(nameof(DeleteUsuario), new { id = usuarioDto.IdUsuario }), "delete_usuario", "DELETE"));
        }

        // POST: api/Usuarios
        /// <summary>
        /// Cria um novo usu�rio.
        /// </summary>
        /// <param name="usuarioCreateDto">Dados para a cria��o do usu�rio.</param>
        /// <response code="201">Usu�rio criado com sucesso. Retorna o usu�rio criado.</response>
        /// <response code="400">Dados inv�lidos para a cria��o do usu�rio.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpPost]
        [ProducesResponseType(typeof(UsuarioGetDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<UsuarioGetDto>> CreateUsuario([FromBody] UsuarioCreateDto usuarioCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var usuarioModel = await _usuarioRepository.Create(usuarioCreateDto);
                var usuarioGetDto = _mapper.Map<UsuarioGetDto>(usuarioModel);
                AddLinksToUsuario(usuarioGetDto);

                return CreatedAtAction(nameof(GetUsuarioById), new { id = usuarioGetDto.IdUsuario }, usuarioGetDto);
            }
            catch (Exception ex)
            {
                // Tratar exce��es espec�ficas do reposit�rio (e.g., e-mail/pessoa j� existe, Pessoa/TipoUsuario n�o encontrado)
                if (ex.Message.Contains("Pessoa n�o encontrada") || 
                    ex.Message.Contains("Tipo de Usu�rio n�o encontrado") ||
                    ex.Message.Contains("e-mail") ||
                    ex.Message.Contains("pessoa"))
                {
                    return BadRequest(ex.Message);
                }
                return StatusCode(500, $"Erro interno ao criar o usu�rio: {ex.Message}");
            }
        }

        // GET: api/Usuarios/getAll
        /// <summary>
        /// Obt�m todos os usu�rios cadastrados.
        /// </summary>
        /// <response code="200">Lista de usu�rios retornada com sucesso.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpGet("getAll")] // Rota alterada
        [ProducesResponseType(typeof(List<UsuarioGetDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<UsuarioGetDto>>> GetAll() // Nome do m�todo alterado
        {
            try
            {
                var usuarios = await _usuarioRepository.GetAll();
                var usuariosGetDto = _mapper.Map<List<UsuarioGetDto>>(usuarios);
                usuariosGetDto.ForEach(AddLinksToUsuario);
                return Ok(usuariosGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Nenhum usu�rio encontrado")) return Ok(new List<UsuarioGetDto>());
                return StatusCode(500, $"Erro interno ao buscar usu�rios: {ex.Message}");
            }
        }

        // GET: api/Usuarios/{id}
        /// <summary>
        /// Obt�m um usu�rio espec�fico pelo seu ID.
        /// </summary>
        /// <param name="id">ID do usu�rio a ser obtido.</param>
        /// <response code="200">Usu�rio retornado com sucesso.</response>
        /// <response code="404">Usu�rio n�o encontrado.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UsuarioGetDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<UsuarioGetDto>> GetUsuarioById(int id)
        {
            try
            {
                var usuario = await _usuarioRepository.GetById(id);
                var usuarioGetDto = _mapper.Map<UsuarioGetDto>(usuario);
                AddLinksToUsuario(usuarioGetDto);
                return Ok(usuarioGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Usu�rio n�o encontrado")) return NotFound(ex.Message);
                return StatusCode(500, $"Erro interno ao buscar o usu�rio: {ex.Message}");
            }
        }
        
        // GET: api/Usuarios/email/{email}
        /// <summary>
        /// Obt�m um usu�rio espec�fico pelo seu endere�o de e-mail.
        /// </summary>
        /// <param name="email">E-mail do usu�rio a ser obtido.</param>
        /// <response code="200">Usu�rio retornado com sucesso.</response>
        /// <response code="404">Usu�rio n�o encontrado.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpGet("email/{email}")]
        [ProducesResponseType(typeof(UsuarioGetDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<UsuarioGetDto>> GetUsuarioByEmail(string email)
        {
            try
            {
                var usuario = await _usuarioRepository.GetByLogin(email);
                 if (usuario == null)
                {
                    return NotFound($"Usu�rio com e-mail {email} n�o encontrado.");
                }
                var usuarioGetDto = _mapper.Map<UsuarioGetDto>(usuario);
                AddLinksToUsuario(usuarioGetDto);
                return Ok(usuarioGetDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao buscar o usu�rio por e-mail: {ex.Message}");
            }
        }


        // PUT: api/Usuarios/{id}
        /// <summary>
        /// Atualiza um usu�rio existente.
        /// </summary>
        /// <param name="id">ID do usu�rio a ser atualizado.</param>
        /// <param name="usuarioUpdateDto">Dados para a atualiza��o do usu�rio.</param>
        /// <response code="200">Usu�rio atualizado com sucesso. Retorna o usu�rio atualizado.</response>
        /// <response code="400">Dados inv�lidos para a atualiza��o.</response>
        /// <response code="404">Usu�rio n�o encontrado.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(UsuarioGetDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<UsuarioGetDto>> UpdateUsuario(int id, [FromBody] UsuarioUpdateDto usuarioUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var usuarioAtualizado = await _usuarioRepository.UpdateById(id, usuarioUpdateDto);
                var usuarioGetDto = _mapper.Map<UsuarioGetDto>(usuarioAtualizado);
                return Ok(usuarioGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Usu�rio n�o encontrado") || ex.Message.Contains("Tipo de Usu�rio n�o encontrado")) return NotFound(ex.Message);
                if (ex.Message.Contains("e-mail")) return BadRequest(ex.Message);
                return StatusCode(500, $"Erro interno ao atualizar o usu�rio: {ex.Message}");
            }
        }

        // DELETE: api/Usuarios/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            try
            {
                var sucesso = await _usuarioRepository.DeleteById(id);
                if (!sucesso)
                {
                    return NotFound($"Usu�rio com ID {id} n�o encontrado.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Usu�rio n�o encontrado")) return NotFound(ex.Message);
                return StatusCode(500, $"Erro interno ao excluir o usu�rio: {ex.Message}");
            }
        }
    }
}