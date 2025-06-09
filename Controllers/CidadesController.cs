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
    /// Gerencia as opera��es relacionadas a cidades.
    /// </summary>
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
        
        private void AddLinksToCidade(CidadeGetDto cidadeDto)
        {
            if (cidadeDto == null) return;
            cidadeDto.Links.Add(new LinkDto(Url.Link(nameof(GetCidadeById), new { id = cidadeDto.IdCidade }), "self", "GET"));
            cidadeDto.Links.Add(new LinkDto(Url.Link(nameof(UpdateCidade), new { id = cidadeDto.IdCidade }), "update_cidade", "PUT"));
            cidadeDto.Links.Add(new LinkDto(Url.Link(nameof(DeleteCidade), new { id = cidadeDto.IdCidade }), "delete_cidade", "DELETE"));
            // Adicionar link para o estado
            if (cidadeDto.Estado != null)
            {
                cidadeDto.Links.Add(new LinkDto(Url.Link("GetEstadoById", new { controller = "Estados", id = cidadeDto.Estado.IdEstado }), "estado", "GET"));
            }
            // Adicionar link para listar endere�os desta cidade, se houver tal endpoint
            // cidadeDto.Links.Add(new LinkDto(Url.Link("GetEnderecosByCidadeId", new { controller = "Enderecos", cidadeId = cidadeDto.IdCidade }), "enderecos", "GET"));
        }

        /// <summary>
        /// Cria uma nova cidade.
        /// </summary>
        /// <param name="cidadeDto">Dados para a cria��o da cidade.</param>
        /// <response code="201">Cidade criada com sucesso. Retorna a cidade criada.</response>
        /// <response code="400">Dados inv�lidos (ex: nome duplicado no mesmo estado, estado n�o encontrado).</response>
        /// <response code="500">Erro interno no servidor.</response>
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
                AddLinksToCidade(cidadeGetDto);
                return CreatedAtAction(nameof(GetCidadeById), new { id = cidadeGetDto.IdCidade }, cidadeGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Estado n�o encontrado") || 
                    ex.Message.Contains("Nome da cidade j� existe"))
                {
                    return BadRequest(ex.Message);
                }
                return StatusCode(500, $"Erro interno ao criar a cidade: {ex.Message}");
            }
        }

        /// <summary>
        /// Obt�m todas as cidades cadastradas.
        /// </summary>
        /// <response code="200">Lista de cidades retornada com sucesso.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpGet("getAll")]
        [ProducesResponseType(typeof(List<CidadeGetDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<CidadeGetDto>>> GetAllCidades()
        {
            try
            {
                var cidades = await _cidadeRepository.GetAll();
                var cidadesGetDto = _mapper.Map<List<CidadeGetDto>>(cidades);
                cidadesGetDto.ForEach(AddLinksToCidade);
                return Ok(cidadesGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Nenhuma cidade encontrada")) return Ok(new List<CidadeGetDto>());
                return StatusCode(500, $"Erro interno ao buscar cidades: {ex.Message}");
            }
        }

        /// <summary>
        /// Obt�m uma cidade espec�fica pelo seu ID.
        /// </summary>
        /// <param name="id">ID da cidade.</param>
        /// <response code="200">Cidade retornada com sucesso.</response>
        /// <response code="404">Cidade n�o encontrada.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CidadeGetDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<CidadeGetDto>> GetCidadeById(int id)
        {
            try
            {
                var cidade = await _cidadeRepository.GetById(id);
                var cidadeGetDto = _mapper.Map<CidadeGetDto>(cidade);
                AddLinksToCidade(cidadeGetDto);
                return Ok(cidadeGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Cidade n�o encontrada")) return NotFound(ex.Message);
                return StatusCode(500, $"Erro interno ao buscar a cidade: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Obt�m todas as cidades de um estado espec�fico.
        /// </summary>
        /// <param name="estadoId">ID do estado.</param>
        /// <response code="200">Lista de cidades do estado retornada com sucesso.</response>
        /// <response code="404">Estado n�o encontrado.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpGet("estado/{estadoId}")]
        [ProducesResponseType(typeof(List<CidadeGetDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<CidadeGetDto>>> GetCidadesByEstadoId(int estadoId)
        {
            try
            {
                var cidades = await _cidadeRepository.GetByEstadoId(estadoId);
                var cidadesGetDto = _mapper.Map<List<CidadeGetDto>>(cidades);
                cidadesGetDto.ForEach(AddLinksToCidade);
                return Ok(cidadesGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Estado n�o encontrado")) return NotFound(ex.Message);
                if (ex.Message.Contains("Nenhuma cidade encontrada para o estado")) return Ok(new List<CidadeGetDto>());
                return StatusCode(500, $"Erro interno ao buscar cidades do estado: {ex.Message}");
            }
        }

        /// <summary>
        /// Atualiza uma cidade existente.
        /// </summary>
        /// <param name="id">ID da cidade a ser atualizada.</param>
        /// <param name="cidadeDto">Dados para a atualiza��o.</param>
        /// <response code="200">Cidade atualizada com sucesso. Retorna a cidade atualizada.</response>
        /// <response code="400">Dados inv�lidos (ex: nome duplicado, estado n�o encontrado).</response>
        /// <response code="404">Cidade n�o encontrada.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(CidadeGetDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<CidadeGetDto>> UpdateCidade(int id, [FromBody] CidadeDto cidadeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var cidadeAtualizada = await _cidadeRepository.Update(cidadeDto, id);
                var cidadeGetDto = _mapper.Map<CidadeGetDto>(cidadeAtualizada);
                AddLinksToCidade(cidadeGetDto);
                return Ok(cidadeGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Cidade n�o encontrada")) return NotFound(ex.Message);
                if (ex.Message.Contains("Estado n�o encontrado") || 
                    ex.Message.Contains("Nome da cidade j� existe"))
                {
                    return BadRequest(ex.Message);
                }
                return StatusCode(500, $"Erro interno ao atualizar a cidade: {ex.Message}");
            }
        }

        /// <summary>
        /// Exclui uma cidade.
        /// </summary>
        /// <param name="id">ID da cidade a ser exclu�da.</param>
        /// <response code="204">Cidade exclu�da com sucesso.</response>
        /// <response code="400">N�o � poss�vel excluir a cidade pois ela est� associada a endere�os.</response>
        /// <response code="404">Cidade n�o encontrada.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteCidade(int id)
        {
            try
            {
                var sucesso = await _cidadeRepository.Delete(id);
                if (!sucesso)
                {
                    return NotFound($"Cidade com ID {id} n�o encontrada.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Cidade n�o encontrada")) return NotFound(ex.Message);
                if (ex.Message.Contains("associada a endere�os")) return BadRequest(ex.Message);
                return StatusCode(500, $"Erro interno ao excluir a cidade: {ex.Message}");
            }
        }
    }
}