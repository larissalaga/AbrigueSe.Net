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
    /// Gerencia as operações relacionadas a abrigos.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AbrigosController : ControllerBase
    {
        private readonly IAbrigoRepository _abrigoRepository;
        private readonly IMapper _mapper;

        public AbrigosController(IAbrigoRepository abrigoRepository, IMapper mapper)
        {
            _abrigoRepository = abrigoRepository;
            _mapper = mapper;
        }

        private void AddLinksToAbrigo(AbrigoGetDto abrigoDto)
        {
            if (abrigoDto == null) return;

            abrigoDto.Links.Add(new LinkDto(Url.Link(nameof(GetAbrigoById), new { id = abrigoDto.IdAbrigo }), "self", "GET"));
            abrigoDto.Links.Add(new LinkDto(Url.Link(nameof(GetAbrigoDetailsById), new { id = abrigoDto.IdAbrigo }), "details", "GET"));
            abrigoDto.Links.Add(new LinkDto(Url.Link(nameof(UpdateAbrigo), new { id = abrigoDto.IdAbrigo }), "update_abrigo", "PUT"));
            abrigoDto.Links.Add(new LinkDto(Url.Link(nameof(DeleteAbrigo), new { id = abrigoDto.IdAbrigo }), "delete_abrigo", "DELETE"));
            // Adicionar links para recursos relacionados, como endereço, se aplicável
            if (abrigoDto.Endereco != null)
            {
                // Assumindo que existe um EnderecosController com GetEnderecoById
                // abrigoDto.Links.Add(new LinkDto(Url.Link("GetEnderecoById", new { controller = "Enderecos", id = abrigoDto.Endereco.IdEndereco }), "endereco", "GET"));
            }
        }

        private void AddLinksToAbrigo(Abrigo abrigo) // Sobrecarga para o tipo Abrigo
        {
            if (abrigo == null) return;

            // Para o tipo Abrigo, que não tem a lista de Links diretamente, precisaríamos mapeá-lo para AbrigoGetDto primeiro
            // ou adicionar links de uma forma diferente se AbrigoGetDto não for sempre o tipo de retorno.
            // Por simplicidade, vamos assumir que sempre podemos mapear para AbrigoGetDto ou que o cliente espera um AbrigoGetDto com links.
            // Se o método GetAll retorna List<Abrigo> e não List<AbrigoGetDto>,
            // então os links HATEOAS não seriam adicionados a menos que você mapeie cada Abrigo para AbrigoGetDto.
        }

        // POST: api/Abrigos
        /// <summary>
        /// Cria um novo abrigo.
        /// </summary>
        /// <param name="abrigoDto">Dados para a criação do abrigo.</param>
        /// <response code="201">Abrigo criado com sucesso. Retorna o abrigo criado.</response>
        /// <response code="400">Dados inválidos para a criação do abrigo.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpPost]
        [ProducesResponseType(typeof(AbrigoGetDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<AbrigoGetDto>> CreateAbrigo([FromBody] AbrigoCreateDto abrigoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var abrigoModel = await _abrigoRepository.Create(abrigoDto);
                var abrigoGetDto = _mapper.Map<AbrigoGetDto>(abrigoModel);
                AddLinksToAbrigo(abrigoGetDto);

                return CreatedAtAction(nameof(GetAbrigoById), new { id = abrigoGetDto.IdAbrigo }, abrigoGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("nome")) return BadRequest(ex.Message); // Erro de nome duplicado
                return StatusCode(500, $"Erro interno ao criar o abrigo: {ex.Message}");
            }
        }

        // GET: api/Abrigos/getAll
        /// <summary>
        /// Obtém todos os abrigos cadastrados.
        /// </summary>
        /// <response code="200">Lista de abrigos retornada com sucesso.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpGet("getAll")]
        [ProducesResponseType(typeof(List<AbrigoGetDto>), 200)] // Alterado para AbrigoGetDto
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<AbrigoGetDto>>> GetAll() // Alterado para AbrigoGetDto
        {
            try
            {
                var abrigos = await _abrigoRepository.GetAll();
                var abrigosGetDto = _mapper.Map<List<AbrigoGetDto>>(abrigos); // Mapear para DTO
                abrigosGetDto.ForEach(AddLinksToAbrigo); // Adicionar links
                return Ok(abrigosGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Nenhum abrigo encontrado")) return Ok(new List<AbrigoGetDto>());
                return StatusCode(500, $"Erro interno ao buscar abrigos: {ex.Message}");
            }
        }

        // GET: api/Abrigos/{id}
        /// <summary>
        /// Obtém um abrigo específico pelo seu ID.
        /// </summary>
        /// <param name="id">ID do abrigo a ser obtido.</param>
        /// <response code="200">Abrigo retornado com sucesso.</response>
        /// <response code="404">Abrigo não encontrado.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AbrigoGetDto), 200)] // Alterado para AbrigoGetDto
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<AbrigoGetDto>> GetAbrigoById(int id) // Alterado para AbrigoGetDto
        {
            try
            {
                var abrigo = await _abrigoRepository.GetById(id);
                var abrigoGetDto = _mapper.Map<AbrigoGetDto>(abrigo); // Mapear para DTO
                AddLinksToAbrigo(abrigoGetDto); // Adicionar links
                return Ok(abrigoGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Abrigo não encontrado")) return NotFound(ex.Message);
                return StatusCode(500, $"Erro interno ao buscar o abrigo: {ex.Message}");
            }
        }

        // GET: api/Abrigos/{id}/details
        /// <summary>
        /// Obtém detalhes de um abrigo específico, incluindo informações relacionadas.
        /// </summary>
        /// <param name="id">ID do abrigo para obter detalhes.</param>
        /// <response code="200">Detalhes do abrigo retornados com sucesso.</response>
        /// <response code="404">Abrigo não encontrado.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpGet("{id}/details")]
        [ProducesResponseType(typeof(AbrigoGetDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<AbrigoGetDto>> GetAbrigoDetailsById(int id)
        {
            try
            {
                var abrigoGetDto = await _abrigoRepository.GetDetailsByIdAsync(id);
                AddLinksToAbrigo(abrigoGetDto);
                return Ok(abrigoGetDto);
            }
            catch (KeyNotFoundException knfex)
            {
                return NotFound(knfex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao buscar detalhes do abrigo: {ex.Message}");
            }
        }

        // PUT: api/Abrigos/{id}
        /// <summary>
        /// Atualiza um abrigo existente.
        /// </summary>
        /// <param name="id">ID do abrigo a ser atualizado.</param>
        /// <param name="abrigoDto">Dados para a atualização do abrigo.</param>
        /// <response code="200">Abrigo atualizado com sucesso. Retorna o abrigo atualizado.</response>
        /// <response code="400">Dados inválidos para a atualização.</response>
        /// <response code="404">Abrigo não encontrado.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(AbrigoGetDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<AbrigoGetDto>> UpdateAbrigo(int id, [FromBody] AbrigoCreateDto abrigoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var abrigoAtualizado = await _abrigoRepository.UpdateById(id, abrigoDto);
                // O repositório já retorna AbrigoGetDto neste caso, conforme a assinatura de UpdateById.
                // Se retornasse Abrigo, seria necessário mapear: _mapper.Map<AbrigoGetDto>(abrigoAtualizado);
                AddLinksToAbrigo(abrigoAtualizado);
                return Ok(abrigoAtualizado);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Abrigo não encontrado")) return NotFound(ex.Message);
                if (ex.Message.Contains("nome")) return BadRequest(ex.Message); // Erro de nome duplicado
                return StatusCode(500, $"Erro interno ao atualizar o abrigo: {ex.Message}");
            }
        }

        // DELETE: api/Abrigos/{id}
        /// <summary>
        /// Exclui um abrigo existente.
        /// </summary>
        /// <param name="id">ID do abrigo a ser excluído.</param>
        /// <response code="204">Abrigo excluído com sucesso.</response>
        /// <response code="400">Não é possível excluir o abrigo devido a restrições (ex: check-ins ativos).</response>
        /// <response code="404">Abrigo não encontrado.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)] // Para FK constraints
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteAbrigo(int id)
        {
            try
            {
                // Adicionar verificação no repositório se o abrigo tem check-ins ou estoque antes de excluir
                var sucesso = await _abrigoRepository.DeleteById(id);
                if (!sucesso) // O repo já lança exceção se não encontrar
                {
                     return NotFound($"Abrigo com ID {id} não encontrado.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Abrigo não encontrado")) return NotFound(ex.Message);
                // Adicionar tratamento para FKs se o repositório lançar exceções específicas
                // Ex: if (ex.Message.Contains("check-ins ativos")) return BadRequest("Este abrigo possui check-ins ativos e não pode ser excluído.");
                return StatusCode(500, $"Erro interno ao excluir o abrigo: {ex.Message}");
            }
        }
    }
}