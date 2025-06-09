using AbrigueSe.Models;
using AbrigueSe.MlModels; // Namespace do GenerativeAIService
using AbrigueSe.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using AbrigueSe.Dtos; // Adicionado para AnalysisResultDto e LinkDto
using System.Collections.Generic; // Adicionado para List

namespace AbrigueSe.Controllers
{
    /// <summary>
    /// Controller respons�vel por fornecer an�lises utilizando Intelig�ncia Artificial.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AIAnalysisController : ControllerBase
    {
        private readonly IAbrigoRepository _abrigoRepository;
        private readonly IEstoqueRecursoRepository _estoqueRecursoRepository;
        private readonly IPessoaRepository _pessoaRepository; // Adicionado
        private readonly GenerativeAIService _generativeAIService; // Injetar o servi�o de IA

        public AIAnalysisController(
            IAbrigoRepository abrigoRepository,
            IEstoqueRecursoRepository estoqueRecursoRepository,
            IPessoaRepository pessoaRepository, // Adicionado
            GenerativeAIService generativeAIService)
        {
            _abrigoRepository = abrigoRepository;
            _estoqueRecursoRepository = estoqueRecursoRepository;
            _pessoaRepository = pessoaRepository; // Adicionado
            _generativeAIService = generativeAIService;
        }

        private void AddLinksToAnalysis(AnalysisResultDto analysisDto, int idAbrigo)
        {
            if (analysisDto == null) return;
            analysisDto.Links.Add(new LinkDto(Url.Link(nameof(GetInventoryAnalysis), new { idAbrigo = idAbrigo }), "self", "GET"));
        }

        private void AddLinksToHealthAnalysis(AnalysisResultDto analysisDto, int idAbrigo)
        {
            if (analysisDto == null) return;
            analysisDto.Links.Add(new LinkDto(Url.Link(nameof(GetShelterHealthAnalysis), new { idAbrigo = idAbrigo }), "self", "GET"));
        }

        /// <summary>
        /// Gera uma an�lise de invent�rio para um abrigo espec�fico utilizando IA.
        /// </summary>
        /// <remarks>
        /// A an�lise considera o n�mero atual de ocupantes e os recursos em estoque,
        /// estimando a autonomia e identificando itens cr�ticos.
        /// </remarks>
        /// <param name="idAbrigo">ID do abrigo para o qual a an�lise ser� gerada.</param>
        /// <response code="200">An�lise de invent�rio gerada com sucesso. Retorna a an�lise em formato de texto.</response>
        /// <response code="404">Abrigo com o ID especificado n�o encontrado.</response>
        /// <response code="500">Erro interno no servidor ao tentar gerar a an�lise ou comunicar com o servi�o de IA.</response>
        [HttpGet("abrigo/{idAbrigo}/analise-estoque")]
        [ProducesResponseType(typeof(AnalysisResultDto), 200)]
        [ProducesResponseType(typeof(string), 404)] // Mensagem de erro como string
        [ProducesResponseType(typeof(string), 500)] // Mensagem de erro como string
        public async Task<ActionResult<AnalysisResultDto>> GetInventoryAnalysis(int idAbrigo)
        {
            try
            {
                // 1. Buscar o Abrigo
                var abrigo = await _abrigoRepository.GetById(idAbrigo);

                if (abrigo == null) 
                {
                    return NotFound($"Abrigo com ID {idAbrigo} n�o encontrado.");
                }

                // 2. Buscar o Estoque de Recursos para o Abrigo
                var estoqueRecursos = await _estoqueRecursoRepository.GetByAbrigoId(idAbrigo);


                // 3. Chamar o servi�o de IA
                var analysisText = await _generativeAIService.GenerateInventoryAnalisys(abrigo, estoqueRecursos);

                var analysisResultDto = new AnalysisResultDto { Analysis = analysisText };
                AddLinksToAnalysis(analysisResultDto, idAbrigo);

                return Ok(analysisResultDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro em GetInventoryAnalysis para o abrigo {idAbrigo}: {ex}");
                if (ex.Message.Contains("n�o encontrado")) 
                {
                    return NotFound(ex.Message);
                }
                return StatusCode(500, $"Erro interno ao gerar an�lise de invent�rio: {ex.Message}");
            }
        }

        /// <summary>
        /// Gera uma an�lise de sa�de agregada e anonimizada para as pessoas em um abrigo espec�fico, utilizando IA.
        /// </summary>
        /// <remarks>
        /// A an�lise considera as condi��es m�dicas informadas pelas pessoas abrigadas para identificar
        /// padr�es de sa�de, riscos potenciais e sugerir a��es preventivas para os gestores do abrigo.
        /// A privacidade � mantida atrav�s da anonimiza��o dos dados enviados � IA.
        /// </remarks>
        /// <param name="idAbrigo">ID do abrigo para o qual a an�lise de sa�de ser� gerada.</param>
        /// <response code="200">An�lise de sa�de gerada com sucesso. Retorna a an�lise em formato de texto.</response>
        /// <response code="404">Abrigo com o ID especificado n�o encontrado.</response>
        /// <response code="500">Erro interno no servidor ao tentar gerar a an�lise ou comunicar com o servi�o de IA.</response>
        [HttpGet("abrigo/{idAbrigo}/analise-saude")]
        [ProducesResponseType(typeof(AnalysisResultDto), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<ActionResult<AnalysisResultDto>> GetShelterHealthAnalysis(int idAbrigo)
        {
            try
            {
                // 1. Buscar o Abrigo
                var abrigo = await _abrigoRepository.GetById(idAbrigo);
                if (abrigo == null)
                {
                    return NotFound($"Abrigo com ID {idAbrigo} n�o encontrado.");
                }

                // 2. Buscar Pessoas (modelo) com check-in ativo no abrigo
                var pessoasNoAbrigo = await _abrigoRepository.GetPessoasAtivasByAbrigoIdAsync(idAbrigo);

                // O GenerativeAIService.GenerateShelterHealthAnalysis j� trata o caso de lista de pessoas vazia.
                // if (!pessoasNoAbrigo.Any())
                // {
                //    return Ok(new AnalysisResultDto { Analysis = "Nenhuma pessoa com check-in ativo encontrada no abrigo para an�lise de sa�de." });
                // }
                
                // 3. Chamar o servi�o de IA
                var healthAnalysisText = await _generativeAIService.GenerateShelterHealthAnalysis(abrigo, pessoasNoAbrigo);

                var analysisResultDto = new AnalysisResultDto { Analysis = healthAnalysisText };
                AddLinksToHealthAnalysis(analysisResultDto, idAbrigo);

                return Ok(analysisResultDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro em GetShelterHealthAnalysis para o abrigo {idAbrigo}: {ex}");
                // N�o retornar NotFound para "Pessoa n�o encontrada" aqui, pois o servi�o de IA deve lidar com lista vazia.
                // Apenas NotFound para o Abrigo.
                return StatusCode(500, $"Erro interno ao gerar an�lise de sa�de: {ex.Message}");
            }
        }
    }
}