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
    /// Gerencia as operações relacionadas a países.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PaisesController : ControllerBase
    {
        private readonly IPaisRepository _paisRepository;
        private readonly IMapper _mapper;

        public PaisesController(IPaisRepository paisRepository, IMapper mapper)
        {
            _paisRepository = paisRepository;
            _mapper = mapper;
        }

        private void AddLinksToPais(PaisGetDto paisDto)
        {
            if (paisDto == null) return;
            paisDto.Links.Add(new LinkDto(Url.Link(nameof(GetPaisById), new { id = paisDto.IdPais }), "self", "GET"));
            paisDto.Links.Add(new LinkDto(Url.Link(nameof(UpdatePais), new { id = paisDto.IdPais }), "update_pais", "PUT"));
            paisDto.Links.Add(new LinkDto(Url.Link(nameof(DeletePais), new { id = paisDto.IdPais }), "delete_pais", "DELETE"));
            // Adicionar link para listar estados deste país, se houver tal endpoint
            // paisDto.Links.Add(new LinkDto(Url.Link("GetEstadosByPaisId", new { controller = "Estados", paisId = paisDto.IdPais }), "estados", "GET"));
        }

        /// <summary>
        /// Cria um novo país.
        /// </summary>
        /// <param name="paisCreateDto">Dados para a criação do país.</param>
        /// <response code="201">País criado com sucesso. Retorna o país criado.</response>
        /// <response code="400">Dados inválidos (ex: nome ou sigla duplicada).</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpPost]
        [ProducesResponseType(typeof(PaisGetDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PaisGetDto>> CreatePais([FromBody] PaisCreateDto paisCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var paisModel = await _paisRepository.Create(paisCreateDto);
                var paisGetDto = _mapper.Map<PaisGetDto>(paisModel);
                AddLinksToPais(paisGetDto);
                return CreatedAtAction(nameof(GetPaisById), new { id = paisGetDto.IdPais }, paisGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Nome do país já existe") || ex.Message.Contains("Sigla do país já existe"))
                {
                    return BadRequest(ex.Message);
                }
                return StatusCode(500, $"Erro interno ao criar o país: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtém todos os países cadastrados.
        /// </summary>
        /// <response code="200">Lista de países retornada com sucesso.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpGet("getAll")]
        [ProducesResponseType(typeof(List<PaisGetDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<PaisGetDto>>> GetAllPaises()
        {
            try
            {
                var paises = await _paisRepository.GetAll();
                var paisesGetDto = _mapper.Map<List<PaisGetDto>>(paises);
                paisesGetDto.ForEach(AddLinksToPais);
                return Ok(paisesGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Nenhum país encontrado")) return Ok(new List<PaisGetDto>());
                return StatusCode(500, $"Erro interno ao buscar países: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtém um país específico pelo seu ID.
        /// </summary>
        /// <param name="id">ID do país.</param>
        /// <response code="200">País retornado com sucesso.</response>
        /// <response code="404">País não encontrado.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PaisGetDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PaisGetDto>> GetPaisById(int id)
        {
            try
            {
                var pais = await _paisRepository.GetById(id);
                var paisGetDto = _mapper.Map<PaisGetDto>(pais);
                AddLinksToPais(paisGetDto);
                return Ok(paisGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("País não encontrado")) return NotFound(ex.Message);
                return StatusCode(500, $"Erro interno ao buscar o país: {ex.Message}");
            }
        }

        /// <summary>
        /// Atualiza um país existente.
        /// </summary>
        /// <param name="id">ID do país a ser atualizado.</param>
        /// <param name="paisUpdateDto">Dados para a atualização.</param>
        /// <response code="200">País atualizado com sucesso. Retorna o país atualizado.</response>
        /// <response code="400">Dados inválidos (ex: nome ou sigla duplicada).</response>
        /// <response code="404">País não encontrado.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(PaisGetDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PaisGetDto>> UpdatePais(int id, [FromBody] PaisUpdateDto paisUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var paisAtualizado = await _paisRepository.Update(paisUpdateDto, id);
                var paisGetDto = _mapper.Map<PaisGetDto>(paisAtualizado);
                AddLinksToPais(paisGetDto);
                return Ok(paisGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("País não encontrado")) return NotFound(ex.Message);
                if (ex.Message.Contains("Nome do país já existe") || ex.Message.Contains("Sigla do país já existe"))
                {
                    return BadRequest(ex.Message);
                }
                return StatusCode(500, $"Erro interno ao atualizar o país: {ex.Message}");
            }
        }

        /// <summary>
        /// Exclui um país.
        /// </summary>
        /// <param name="id">ID do país a ser excluído.</param>
        /// <response code="204">País excluído com sucesso.</response>
        /// <response code="400">Não é possível excluir o país pois ele está associado a estados.</response>
        /// <response code="404">País não encontrado.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeletePais(int id)
        {
            try
            {
                var sucesso = await _paisRepository.Delete(id);
                if (!sucesso)
                {
                    return NotFound($"País com ID {id} não encontrado.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("País não encontrado")) return NotFound(ex.Message);
                if (ex.Message.Contains("associado a estados")) return BadRequest(ex.Message);
                return StatusCode(500, $"Erro interno ao excluir o país: {ex.Message}");
            }
        }
    }
}