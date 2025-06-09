using AbrigueSe.Dtos;
using AbrigueSe.Models;
using AbrigueSe.Repositories.Implementations;
using AbrigueSe.Repositories.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AbrigueSe.Controllers
{
    /// <summary>
    /// Gerencia as operações relacionadas a endereços.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class EnderecosController : ControllerBase
    {
        private readonly IEnderecoRepository _enderecoRepository;
        private readonly IMapper _mapper;

        public EnderecosController(IEnderecoRepository enderecoRepository, IMapper mapper)
        {
            _enderecoRepository = enderecoRepository;
            _mapper = mapper;
        }

        private void AddLinksToEndereco(EnderecoGetDto enderecoDto)
        {
            if (enderecoDto == null) return;

            enderecoDto.Links.Add(new LinkDto(Url.Link(nameof(GetEnderecoById), new { id = enderecoDto.IdEndereco }), "self", "GET"));
            enderecoDto.Links.Add(new LinkDto(Url.Link(nameof(UpdateEndereco), new { id = enderecoDto.IdEndereco }), "update_endereco", "PUT"));
            enderecoDto.Links.Add(new LinkDto(Url.Link(nameof(DeleteEndereco), new { id = enderecoDto.IdEndereco }), "delete_endereco", "DELETE"));
            // Adicionar links para cidade, estado, país, se aplicável e se houver controllers para eles
        }

        // POST: api/Enderecos
        /// <summary>
        /// Cria um novo endereço.
        /// </summary>
        /// <param name="enderecoDto">Dados para a criação do endereço.</param>
        /// <response code="201">Endereço criado com sucesso. Retorna o endereço criado.</response>
        /// <response code="400">Dados inválidos para a criação do endereço.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpPost]
        [ProducesResponseType(typeof(EnderecoGetDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<EnderecoGetDto>> CreateEndereco([FromBody] EnderecoDto enderecoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {   
                var enderecoModel = await _enderecoRepository.Create(enderecoDto);
                var enderecoGetDto = _mapper.Map<EnderecoGetDto>(enderecoModel);
                AddLinksToEndereco(enderecoGetDto);

                return CreatedAtAction(nameof(GetEnderecoById), new { id = enderecoGetDto.IdEndereco }, enderecoGetDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao criar o endereço: {ex.Message}");
            }
        }

        // GET: api/Enderecos/getAll
        /// <summary>
        /// Obtém todos os endereços cadastrados.
        /// </summary>
        /// <response code="200">Lista de endereços retornada com sucesso.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpGet("getAll")] // Rota alterada
        [ProducesResponseType(typeof(List<EnderecoGetDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<EnderecoGetDto>>> GetAll() // Nome do método alterado
        {
            try
            {
                var enderecos = await _enderecoRepository.GetAll();
                var enderecosGetDto = _mapper.Map<List<EnderecoGetDto>>(enderecos);
                enderecosGetDto.ForEach(AddLinksToEndereco);
                return Ok(enderecosGetDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao buscar endereços: {ex.Message}");
            }
        }

        // GET: api/Enderecos/{id}
        /// <summary>
        /// Obtém um endereço específico pelo seu ID.
        /// </summary>
        /// <param name="id">ID do endereço a ser obtido.</param>
        /// <response code="200">Endereço retornado com sucesso.</response>
        /// <response code="404">Endereço não encontrado.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EnderecoGetDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<EnderecoGetDto>> GetEnderecoById(int id)
        {
            try
            {
                var endereco = await _enderecoRepository.GetById(id);
                if (endereco == null)
                {
                    return NotFound($"Endereço com ID {id} não encontrado.");
                }
                var enderecoGetDto = _mapper.Map<EnderecoGetDto>(endereco);
                AddLinksToEndereco(enderecoGetDto);
                return Ok(enderecoGetDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao buscar o endereço: {ex.Message}");
            }
        }

        // PUT: api/Enderecos/{id}
        /// <summary>
        /// Atualiza um endereço existente.
        /// </summary>
        /// <param name="id">ID do endereço a ser atualizado.</param>
        /// <param name="enderecoDto">Dados para a atualização do endereço.</param>
        /// <response code="200">Endereço atualizado com sucesso. Retorna o endereço atualizado.</response>
        /// <response code="400">Dados inválidos para a atualização.</response>
        /// <response code="404">Endereço não encontrado.</response>
        /// <response code="409">Conflito de concorrência ao atualizar o endereço.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(EnderecoGetDto), 200)] // Alterado de 204 para 200 para retornar o objeto atualizado
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateEndereco(int id, [FromBody] EnderecoDto enderecoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var enderecoExistente = await _enderecoRepository.GetById(id);
                if (enderecoExistente == null)
                {
                    return NotFound($"Endereço com ID {id} não encontrado para atualização.");
                }

                var enderecoAtualizado = await _enderecoRepository.Update(enderecoDto, id);
                var enderecoGetDto = _mapper.Map<EnderecoGetDto>(enderecoAtualizado);
                AddLinksToEndereco(enderecoGetDto);
                return Ok(enderecoGetDto);
            }
            catch (DbUpdateConcurrencyException)
            {
                 return StatusCode(409, "Conflito de concorrência ao atualizar o endereço. Tente novamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao atualizar o endereço: {ex.Message}");
            }
        }

        // DELETE: api/Enderecos/{id}
        /// <summary>
        /// Exclui um endereço existente.
        /// </summary>
        /// <param name="id">ID do endereço a ser excluído.</param>
        /// <response code="204">Endereço excluído com sucesso.</response>
        /// <response code="404">Endereço não encontrado.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteEndereco(int id)
        {
            try
            {
                var sucesso = await _enderecoRepository.Delete(id);
                if (!sucesso)
                {
                    return NotFound($"Endereço com ID {id} não encontrado para exclusão.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao excluir o endereço: {ex.Message}");
            }
        }
    }
}