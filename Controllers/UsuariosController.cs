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
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMapper _mapper;

        public UsuariosController(IUsuarioRepository usuarioRepository, IMapper mapper)
        {
            _usuarioRepository = usuarioRepository;
            _mapper = mapper;
        }

        // POST: api/Usuarios
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
        [HttpGet("getAll")] // Rota alterada
        [ProducesResponseType(typeof(List<UsuarioGetDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<UsuarioGetDto>>> GetAll() // Nome do m�todo alterado
        {
            try
            {
                var usuarios = await _usuarioRepository.GetAll();
                var usuariosGetDto = _mapper.Map<List<UsuarioGetDto>>(usuarios);
                return Ok(usuariosGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Nenhum usu�rio encontrado")) return Ok(new List<UsuarioGetDto>());
                return StatusCode(500, $"Erro interno ao buscar usu�rios: {ex.Message}");
            }
        }

        // GET: api/Usuarios/{id}
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
                return Ok(usuarioGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Usu�rio n�o encontrado")) return NotFound(ex.Message);
                return StatusCode(500, $"Erro interno ao buscar o usu�rio: {ex.Message}");
            }
        }
        
        // GET: api/Usuarios/email/{email}
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
                return Ok(usuarioGetDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao buscar o usu�rio por e-mail: {ex.Message}");
            }
        }


        // PUT: api/Usuarios/{id}
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