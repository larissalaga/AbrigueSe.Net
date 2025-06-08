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
    public class PessoasController : ControllerBase
    {
        private readonly IPessoaRepository _pessoaRepository;
        private readonly IMapper _mapper;

        public PessoasController(IPessoaRepository pessoaRepository, IMapper mapper)
        {
            _pessoaRepository = pessoaRepository;
            _mapper = mapper;
        }

        // POST: api/Pessoas
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
        [HttpGet("getAll")] // Rota alterada
        [ProducesResponseType(typeof(List<PessoaGetDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<PessoaGetDto>>> GetAll() // Nome do m�todo alterado
        {
            try
            {
                var pessoas = await _pessoaRepository.GetAll();
                var pessoasGetDto = _mapper.Map<List<PessoaGetDto>>(pessoas);
                return Ok(pessoasGetDto);
            }
            catch (Exception ex)
            {
                 if (ex.Message.Contains("Nenhuma pessoa encontrada")) return Ok(new List<PessoaGetDto>()); // Retorna lista vazia se for essa a exce��o
                return StatusCode(500, $"Erro interno ao buscar pessoas: {ex.Message}");
            }
        }

        // GET: api/Pessoas/{id}
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
        [HttpGet("{id}/details")]
        [ProducesResponseType(typeof(PessoaGetDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PessoaGetDto>> GetPessoaDetailsById(int id)
        {
            try
            {
                var pessoaGetDto = await _pessoaRepository.GetDetailsByIdAsync(id);
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