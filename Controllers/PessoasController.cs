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
    /// Gerencia as opera��es relacionadas a pessoas.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PessoasController : ControllerBase
    {
        private readonly IPessoaRepository _pessoaRepository;
        private readonly IMapper _mapper;

        public PessoasController(IPessoaRepository pessoaRepository, IMapper mapper)
        {
            _pessoaRepository = pessoaRepository;
            _mapper = mapper;
        }

        private void AddLinksToPessoa(PessoaGetDto pessoaDto)
        {
            if (pessoaDto == null) return;

            pessoaDto.Links.Add(new LinkDto(Url.Link(nameof(GetPessoaById), new { id = pessoaDto.IdPessoa }), "self", "GET"));
            pessoaDto.Links.Add(new LinkDto(Url.Link(nameof(GetPessoaDetailsById), new { id = pessoaDto.IdPessoa }), "details", "GET"));
            pessoaDto.Links.Add(new LinkDto(Url.Link(nameof(UpdatePessoa), new { id = pessoaDto.IdPessoa }), "update_pessoa", "PUT"));
            pessoaDto.Links.Add(new LinkDto(Url.Link(nameof(DeletePessoa), new { id = pessoaDto.IdPessoa }), "delete_pessoa", "DELETE"));
            // Adicionar link para endere�o, se aplic�vel
            if (pessoaDto.Endereco != null)
            {
                // Assumindo que existe um EnderecosController com GetEnderecoById
                // pessoaDto.Links.Add(new LinkDto(Url.Link("GetEnderecoById", new { controller = "Enderecos", id = pessoaDto.Endereco.IdEndereco }), "endereco", "GET"));
            }
            // Adicionar link para abrigo atual, se aplic�vel
            if (pessoaDto.AbrigoAtual != null)
            {
                // pessoaDto.Links.Add(new LinkDto(Url.Link("GetAbrigoById", new { controller = "Abrigos", id = pessoaDto.AbrigoAtual.IdAbrigo }), "abrigo_atual", "GET"));
            }
        }

        // POST: api/Pessoas
        /// <summary>
        /// Cria uma nova pessoa.
        /// </summary>
        /// <param name="pessoaDto">Dados para a cria��o da pessoa.</param>
        /// <response code="201">Pessoa criada com sucesso. Retorna a pessoa criada.</response>
        /// <response code="400">Dados inv�lidos para a cria��o da pessoa (ex: CPF duplicado, endere�o n�o encontrado).</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpPost]
        [ProducesResponseType(typeof(PessoaGetDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PessoaGetDto>> CreatePessoa([FromBody] PessoaDto pessoaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var pessoaModel = await _pessoaRepository.Create(pessoaDto); // Reposit�rio j� retorna o modelo com ID
                var pessoaGetDto = _mapper.Map<PessoaGetDto>(pessoaModel);
                AddLinksToPessoa(pessoaGetDto);
                // O abrigo atual ser� populado pelo reposit�rio e mapeado pelo AutoMapper
                return CreatedAtAction(nameof(GetPessoaById), new { id = pessoaGetDto.IdPessoa }, pessoaGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("CPF") || ex.Message.Contains("Endere�o n�o encontrado")) // Exemplo de tratamento de erro espec�fico do repo
                {
                    return BadRequest(ex.Message);
                }
                return StatusCode(500, $"Erro interno ao criar a pessoa: {ex.Message}");
            }
        }

        // GET: api/Pessoas/getAll
        /// <summary>
        /// Obt�m todas as pessoas cadastradas.
        /// </summary>
        /// <response code="200">Lista de pessoas retornada com sucesso.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpGet("getAll")] // Rota alterada
        [ProducesResponseType(typeof(List<PessoaGetDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<PessoaGetDto>>> GetAll() // Nome do m�todo alterado
        {
            try
            {
                var pessoas = await _pessoaRepository.GetAll();
                var pessoasGetDto = _mapper.Map<List<PessoaGetDto>>(pessoas);
                pessoasGetDto.ForEach(AddLinksToPessoa);
                return Ok(pessoasGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Nenhuma pessoa encontrada")) return Ok(new List<PessoaGetDto>()); // Retorna lista vazia se for essa a exce��o
                return StatusCode(500, $"Erro interno ao buscar pessoas: {ex.Message}");
            }
        }

        // GET: api/Pessoas/{id}
        /// <summary>
        /// Obt�m uma pessoa espec�fica pelo seu ID.
        /// </summary>
        /// <param name="id">ID da pessoa a ser obtida.</param>
        /// <response code="200">Pessoa retornada com sucesso.</response>
        /// <response code="404">Pessoa n�o encontrada.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PessoaGetDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PessoaGetDto>> GetPessoaById(int id)
        {
            try
            {
                var pessoa = await _pessoaRepository.GetById(id);
                // Reposit�rio j� lan�a exce��o se n�o encontrar, ent�o n�o precisa checar null aqui.
                var pessoaGetDto = _mapper.Map<PessoaGetDto>(pessoa);
                AddLinksToPessoa(pessoaGetDto);
                return Ok(pessoaGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Pessoa n�o encontrada"))
                {
                    return NotFound(ex.Message);
                }
                return StatusCode(500, $"Erro interno ao buscar a pessoa: {ex.Message}");
            }
        }

        // GET: api/Pessoas/{id}/details
        /// <summary>
        /// Obt�m detalhes de uma pessoa espec�fica, incluindo informa��es relacionadas como endere�o e abrigo atual.
        /// </summary>
        /// <param name="id">ID da pessoa para obter detalhes.</param>
        /// <response code="200">Detalhes da pessoa retornados com sucesso.</response>
        /// <response code="404">Pessoa n�o encontrada.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpGet("{id}/details")]
        [ProducesResponseType(typeof(PessoaGetDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PessoaGetDto>> GetPessoaDetailsById(int id)
        {
            try
            {
                var pessoaGetDto = await _pessoaRepository.GetDetailsByIdAsync(id);
                AddLinksToPessoa(pessoaGetDto);
                return Ok(pessoaGetDto);
            }
            catch (KeyNotFoundException knfex)
            {
                return NotFound(knfex.Message);
            }
            catch (Exception ex)
            {
                // TODO: Log the exception ex
                return StatusCode(500, $"Erro interno ao buscar detalhes da pessoa: {ex.Message}");
            }
        }

        // PUT: api/Pessoas/{id}
        /// <summary>
        /// Atualiza uma pessoa existente.
        /// </summary>
        /// <param name="id">ID da pessoa a ser atualizada.</param>
        /// <param name="pessoaDto">Dados para a atualiza��o da pessoa.</param>
        /// <response code="200">Pessoa atualizada com sucesso. Retorna a pessoa atualizada.</response>
        /// <response code="400">Dados inv�lidos para a atualiza��o (ex: CPF duplicado).</response>
        /// <response code="404">Pessoa ou endere�o relacionado n�o encontrado.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(PessoaGetDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PessoaGetDto>> UpdatePessoa(int id, [FromBody] PessoaDto pessoaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var pessoaAtualizada = await _pessoaRepository.UpdateById(id, pessoaDto);
                var pessoaGetDto = _mapper.Map<PessoaGetDto>(pessoaAtualizada);
                AddLinksToPessoa(pessoaGetDto);
                return Ok(pessoaGetDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Pessoa n�o encontrada") || ex.Message.Contains("endere�o n�o encontrado"))
                {
                    return NotFound(ex.Message);
                }
                if (ex.Message.Contains("CPF"))
                {
                    return BadRequest(ex.Message);
                }
                return StatusCode(500, $"Erro interno ao atualizar a pessoa: {ex.Message}");
            }
        }

        // DELETE: api/Pessoas/{id}
        /// <summary>
        /// Exclui uma pessoa existente.
        /// </summary>
        /// <param name="id">ID da pessoa a ser exclu�da.</param>
        /// <response code="204">Pessoa exclu�da com sucesso.</response>
        /// <response code="400">N�o � poss�vel excluir a pessoa devido a restri��es (ex: possui check-in ativo ou � um usu�rio).</response>
        /// <response code="404">Pessoa n�o encontrada.</response>
        /// <response code="500">Erro interno no servidor.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)] // Para erros de FK
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeletePessoa(int id)
        {
            try
            {
                var sucesso = await _pessoaRepository.DeleteById(id);
                if (!sucesso) // Embora o repo lance exce��o, uma dupla checagem n�o faz mal.
                {
                    return NotFound($"Pessoa com ID {id} n�o encontrada para exclus�o.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Pessoa n�o encontrada"))
                {
                    return NotFound(ex.Message);
                }
                if (ex.Message.Contains("check-in") || ex.Message.Contains("usu�rio")) // Erros de FK do reposit�rio
                {
                    return BadRequest(ex.Message);
                }
                return StatusCode(500, $"Erro interno ao excluir a pessoa: {ex.Message}");
            }
        }
    }
}