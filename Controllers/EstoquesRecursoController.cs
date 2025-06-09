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
    /// Gerencia o estoque de recursos (suprimentos) nos abrigos.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class EstoquesRecursoController : ControllerBase
    {
        private readonly IEstoqueRecursoRepository _estoqueRepository;
        private readonly IMapper _mapper;

        public EstoquesRecursoController(IEstoqueRecursoRepository estoqueRepository, IMapper mapper)
        {
            _estoqueRepository = estoqueRepository;
            _mapper = mapper;
        }

        private void AddLinksToEstoque(EstoqueRecursoGetDto estoqueDto)
        {
            if (estoqueDto == null) return;
            estoqueDto.Links.Add(new LinkDto(Url.Link(nameof(GetEstoqueById), new { id = estoqueDto.IdEstoque }), "self", "GET"));
            // Adicionar link para atualizar e deletar
            estoqueDto.Links.Add(new LinkDto(Url.Link(nameof(UpdateEstoque), new { id = estoqueDto.IdEstoque }), "update_estoque", "PUT"));
            estoqueDto.Links.Add(new LinkDto(Url.Link(nameof(DeleteEstoque), new { id = estoqueDto.IdEstoque }), "delete_estoque", "DELETE"));
            // Adicionar links para abrigo e recurso, se houver endpoints para eles.
            // Ex: estoqueDto.Links.Add(new LinkDto(Url.Link("GetAbrigoById", new { controller = "Abrigos", id = estoqueDto.IdAbrigo }), "abrigo", "GET"));
            // Ex: estoqueDto.Links.Add(new LinkDto(Url.Link("GetRecursoById", new { controller = "Recursos", id = estoqueDto.IdRecurso }), "recurso", "GET"));
        }

        /// <summary>
        /// Adiciona um recurso ao estoque de um abrigo ou atualiza sua quantidade se já existir.
        /// </summary>
        /// <param name="estoqueCreateDto">Dados do recurso e abrigo para adicionar ao estoque.</param>
        /// <response code="201">Recurso adicionado/atualizado no estoque com sucesso. Retorna o item de estoque.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="404">Abrigo ou Recurso não encontrado.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpPost]
        [ProducesResponseType(typeof(EstoqueRecursoGetDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<EstoqueRecursoGetDto>> CreateEstoque([FromBody] EstoqueRecursoDto estoqueCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var estoqueModel = await _estoqueRepository.Create(estoqueCreateDto);
                var estoqueGetDto = _mapper.Map<EstoqueRecursoGetDto>(estoqueModel);
                AddLinksToEstoque(estoqueGetDto);
                return CreatedAtAction(nameof(GetEstoqueById), new { id = estoqueGetDto.IdEstoque }, estoqueGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("não encontrado")) return NotFound(ex.Message);
                return StatusCode(500, $"Erro interno ao criar/atualizar o estoque: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtém todos os itens de estoque de todos os abrigos.
        /// </summary>
        /// <response code="200">Lista de itens de estoque retornada com sucesso.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpGet("getAll")]
        [ProducesResponseType(typeof(List<EstoqueRecursoGetDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<EstoqueRecursoGetDto>>> GetAllEstoques()
        {
            try
            {
                var estoques = await _estoqueRepository.GetAll();
                var estoquesGetDto = _mapper.Map<List<EstoqueRecursoGetDto>>(estoques);
                estoquesGetDto.ForEach(AddLinksToEstoque);
                return Ok(estoquesGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Nenhum item de estoque encontrado")) return Ok(new List<EstoqueRecursoGetDto>());
                return StatusCode(500, $"Erro interno ao buscar estoques: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtém um item de estoque específico pelo seu ID.
        /// </summary>
        /// <param name="id">ID do item de estoque.</param>
        /// <response code="200">Item de estoque retornado com sucesso.</response>
        /// <response code="404">Item de estoque não encontrado.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EstoqueRecursoGetDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<EstoqueRecursoGetDto>> GetEstoqueById(int id)
        {
            try
            {
                var estoque = await _estoqueRepository.GetById(id);
                var estoqueGetDto = _mapper.Map<EstoqueRecursoGetDto>(estoque);
                AddLinksToEstoque(estoqueGetDto);
                return Ok(estoqueGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Estoque não encontrado")) return NotFound(ex.Message);
                return StatusCode(500, $"Erro interno ao buscar o item de estoque: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Obtém todos os itens de estoque de um abrigo específico.
        /// </summary>
        /// <param name="idAbrigo">ID do abrigo.</param>
        /// <response code="200">Lista de itens de estoque do abrigo retornada com sucesso.</response>
        /// <response code="404">Abrigo não encontrado.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpGet("abrigo/{idAbrigo}")]
        [ProducesResponseType(typeof(List<EstoqueRecursoGetDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<EstoqueRecursoGetDto>>> GetEstoqueByAbrigoId(int idAbrigo)
        {
            try
            {
                var estoques = await _estoqueRepository.GetByAbrigoId(idAbrigo);
                var estoquesGetDto = _mapper.Map<List<EstoqueRecursoGetDto>>(estoques);
                estoquesGetDto.ForEach(AddLinksToEstoque);
                return Ok(estoquesGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Abrigo não encontrado")) return NotFound(ex.Message);
                 if (ex.Message.Contains("Nenhum item de estoque encontrado para o abrigo")) return Ok(new List<EstoqueRecursoGetDto>());
                return StatusCode(500, $"Erro interno ao buscar estoque do abrigo: {ex.Message}");
            }
        }


        /// <summary>
        /// Atualiza a quantidade de um recurso no estoque de um abrigo.
        /// </summary>
        /// <param name="id">ID do item de estoque a ser atualizado.</param>
        /// <param name="estoqueUpdateDto">Dados para atualização da quantidade.</param>
        /// <response code="200">Item de estoque atualizado com sucesso. Retorna o item atualizado.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="404">Item de estoque não encontrado.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(EstoqueRecursoGetDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<EstoqueRecursoGetDto>> UpdateEstoque(int id, [FromBody] EstoqueRecursoDto estoqueUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var estoqueAtualizado = await _estoqueRepository.UpdateById(id, estoqueUpdateDto);
                var estoqueGetDto = _mapper.Map<EstoqueRecursoGetDto>(estoqueAtualizado);
                AddLinksToEstoque(estoqueGetDto);
                return Ok(estoqueGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Estoque não encontrado")) return NotFound(ex.Message);
                return StatusCode(500, $"Erro interno ao atualizar o item de estoque: {ex.Message}");
            }
        }

        /// <summary>
        /// Remove um recurso do estoque de um abrigo.
        /// </summary>
        /// <param name="id">ID do item de estoque a ser removido.</param>
        /// <response code="204">Item de estoque removido com sucesso.</response>
        /// <response code="404">Item de estoque não encontrado.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteEstoque(int id)
        {
            try
            {
                var sucesso = await _estoqueRepository.DeleteById(id);
                if (!sucesso)
                {
                    return NotFound($"Item de estoque com ID {id} não encontrado.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                 if (ex.Message.Contains("Estoque não encontrado")) return NotFound(ex.Message);
                return StatusCode(500, $"Erro interno ao excluir o item de estoque: {ex.Message}");
            }
        }
    }
}