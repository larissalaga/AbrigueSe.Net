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
    /// Gerencia as operações relacionadas a estados (unidades federativas).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class EstadosController : ControllerBase
    {
        private readonly IEstadoRepository _estadoRepository;
        private readonly IMapper _mapper;

        public EstadosController(IEstadoRepository estadoRepository, IMapper mapper)
        {
            _estadoRepository = estadoRepository;
            _mapper = mapper;
        }
        
        private void AddLinksToEstado(EstadoGetDto estadoDto)
        {
            if (estadoDto == null) return;
            estadoDto.Links.Add(new LinkDto(Url.Link(nameof(GetEstadoById), new { id = estadoDto.IdEstado }), "self", "GET"));
            estadoDto.Links.Add(new LinkDto(Url.Link(nameof(UpdateEstado), new { id = estadoDto.IdEstado }), "update_estado", "PUT"));
            estadoDto.Links.Add(new LinkDto(Url.Link(nameof(DeleteEstado), new { id = estadoDto.IdEstado }), "delete_estado", "DELETE"));
            // Adicionar link para o país
            if (estadoDto.Pais != null)
            {
                estadoDto.Links.Add(new LinkDto(Url.Link("GetPaisById", new { controller = "Paises", id = estadoDto.Pais.IdPais }), "pais", "GET"));
            }
            // Adicionar link para listar cidades deste estado
            // estadoDto.Links.Add(new LinkDto(Url.Link("GetCidadesByEstadoId", new { controller = "Cidades", estadoId = estadoDto.IdEstado }), "cidades", "GET"));
        }

        /// <summary>
        /// Cria um novo estado.
        /// </summary>
        /// <param name="estadoDto">Dados para a criação do estado.</param>
        /// <response code="201">Estado criado com sucesso. Retorna o estado criado.</response>
        /// <response code="400">Dados inválidos (ex: nome ou sigla duplicada no mesmo país, país não encontrado).</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpPost]
        [ProducesResponseType(typeof(EstadoGetDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<EstadoGetDto>> CreateEstado([FromBody] EstadoDto estadoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var estadoModel = await _estadoRepository.Create(estadoDto);
                var estadoGetDto = _mapper.Map<EstadoGetDto>(estadoModel);
                AddLinksToEstado(estadoGetDto);
                return CreatedAtAction(nameof(GetEstadoById), new { id = estadoGetDto.IdEstado }, estadoGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("País não encontrado") || 
                    ex.Message.Contains("Nome do estado já existe") ||
                    ex.Message.Contains("Sigla do estado já existe"))
                {
                    return BadRequest(ex.Message);
                }
                return StatusCode(500, $"Erro interno ao criar o estado: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtém todos os estados cadastrados.
        /// </summary>
        /// <response code="200">Lista de estados retornada com sucesso.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpGet("getAll")]
        [ProducesResponseType(typeof(List<EstadoGetDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<EstadoGetDto>>> GetAllEstados()
        {
            try
            {
                var estados = await _estadoRepository.GetAll();
                var estadosGetDto = _mapper.Map<List<EstadoGetDto>>(estados);
                estadosGetDto.ForEach(AddLinksToEstado);
                return Ok(estadosGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Nenhum estado encontrado")) return Ok(new List<EstadoGetDto>());
                return StatusCode(500, $"Erro interno ao buscar estados: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtém um estado específico pelo seu ID.
        /// </summary>
        /// <param name="id">ID do estado.</param>
        /// <response code="200">Estado retornado com sucesso.</response>
        /// <response code="404">Estado não encontrado.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EstadoGetDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<EstadoGetDto>> GetEstadoById(int id)
        {
            try
            {
                var estado = await _estadoRepository.GetById(id);
                var estadoGetDto = _mapper.Map<EstadoGetDto>(estado);
                AddLinksToEstado(estadoGetDto);
                return Ok(estadoGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Estado não encontrado")) return NotFound(ex.Message);
                return StatusCode(500, $"Erro interno ao buscar o estado: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Obtém todos os estados de um país específico.
        /// </summary>
        /// <param name="paisId">ID do país.</param>
        /// <response code="200">Lista de estados do país retornada com sucesso.</response>
        /// <response code="404">País não encontrado.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpGet("pais/{paisId}")]
        [ProducesResponseType(typeof(List<EstadoGetDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<EstadoGetDto>>> GetEstadosByPaisId(int paisId)
        {
            try
            {
                var estados = await _estadoRepository.GetByPaisId(paisId);
                var estadosGetDto = _mapper.Map<List<EstadoGetDto>>(estados);
                estadosGetDto.ForEach(AddLinksToEstado);
                return Ok(estadosGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("País não encontrado")) return NotFound(ex.Message);
                 if (ex.Message.Contains("Nenhum estado encontrado para o país")) return Ok(new List<EstadoGetDto>());
                return StatusCode(500, $"Erro interno ao buscar estados do país: {ex.Message}");
            }
        }

        /// <summary>
        /// Atualiza um estado existente.
        /// </summary>
        /// <param name="id">ID do estado a ser atualizado.</param>
        /// <param name="estadoDto">Dados para a atualização.</param>
        /// <response code="200">Estado atualizado com sucesso. Retorna o estado atualizado.</response>
        /// <response code="400">Dados inválidos (ex: nome ou sigla duplicada, país não encontrado).</response>
        /// <response code="404">Estado não encontrado.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(EstadoGetDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<EstadoGetDto>> UpdateEstado(int id, [FromBody] EstadoDto estadoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var estadoAtualizado = await _estadoRepository.Update(estadoDto, id);
                var estadoGetDto = _mapper.Map<EstadoGetDto>(estadoAtualizado);
                AddLinksToEstado(estadoGetDto);
                return Ok(estadoGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Estado não encontrado")) return NotFound(ex.Message);
                if (ex.Message.Contains("País não encontrado") || 
                    ex.Message.Contains("Nome do estado já existe") ||
                    ex.Message.Contains("Sigla do estado já existe"))
                {
                    return BadRequest(ex.Message);
                }
                return StatusCode(500, $"Erro interno ao atualizar o estado: {ex.Message}");
            }
        }

        /// <summary>
        /// Exclui um estado.
        /// </summary>
        /// <param name="id">ID do estado a ser excluído.</param>
        /// <response code="204">Estado excluído com sucesso.</response>
        /// <response code="400">Não é possível excluir o estado pois ele está associado a cidades.</response>
        /// <response code="404">Estado não encontrado.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteEstado(int id)
        {
            try
            {
                var sucesso = await _estadoRepository.Delete(id);
                if (!sucesso)
                {
                    return NotFound($"Estado com ID {id} não encontrado.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Estado não encontrado")) return NotFound(ex.Message);
                if (ex.Message.Contains("associado a cidades")) return BadRequest(ex.Message);
                return StatusCode(500, $"Erro interno ao excluir o estado: {ex.Message}");
            }
        }
    }
}