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
    /// Controller responsável por fornecer análises utilizando Inteligência Artificial.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AIAnalysisController : ControllerBase
    {
        private readonly IAbrigoRepository _abrigoRepository;
        private readonly IEstoqueRecursoRepository _estoqueRecursoRepository;
        private readonly IPessoaRepository _pessoaRepository; // Adicionado
        private readonly GenerativeAIService _generativeAIService; // Injetar o serviço de IA

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
        /// Gera uma análise de inventário para um abrigo específico utilizando IA.
        /// </summary>
        /// <remarks>
        /// A análise considera o número atual de ocupantes e os recursos em estoque,
        /// estimando a autonomia e identificando itens críticos.
        /// </remarks>
        /// <param name="idAbrigo">ID do abrigo para o qual a análise será gerada.</param>
        /// <response code="200">Análise de inventário gerada com sucesso. Retorna a análise em formato de texto.</response>
        /// <response code="404">Abrigo com o ID especificado não encontrado.</response>
        /// <response code="500">Erro interno no servidor ao tentar gerar a análise ou comunicar com o serviço de IA.</response>
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
                    return NotFound($"Abrigo com ID {idAbrigo} não encontrado.");
                }

                // 2. Buscar o Estoque de Recursos para o Abrigo
                var estoqueRecursos = await _estoqueRecursoRepository.GetByAbrigoId(idAbrigo);


                // 3. Chamar o serviço de IA
                var analysisText = await _generativeAIService.GenerateInventoryAnalisys(abrigo, estoqueRecursos);

                var analysisResultDto = new AnalysisResultDto { Analysis = analysisText };
                AddLinksToAnalysis(analysisResultDto, idAbrigo);

                return Ok(analysisResultDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro em GetInventoryAnalysis para o abrigo {idAbrigo}: {ex}");
                if (ex.Message.Contains("não encontrado")) 
                {
                    return NotFound(ex.Message);
                }
                return StatusCode(500, $"Erro interno ao gerar análise de inventário: {ex.Message}");
            }
        }

        /// <summary>
        /// Gera uma análise de saúde agregada e anonimizada para as pessoas em um abrigo específico, utilizando IA.
        /// </summary>
        /// <remarks>
        /// A análise considera as condições médicas informadas pelas pessoas abrigadas para identificar
        /// padrões de saúde, riscos potenciais e sugerir ações preventivas para os gestores do abrigo.
        /// A privacidade é mantida através da anonimização dos dados enviados à IA.
        /// </remarks>
        /// <param name="idAbrigo">ID do abrigo para o qual a análise de saúde será gerada.</param>
        /// <response code="200">Análise de saúde gerada com sucesso. Retorna a análise em formato de texto.</response>
        /// <response code="404">Abrigo com o ID especificado não encontrado.</response>
        /// <response code="500">Erro interno no servidor ao tentar gerar a análise ou comunicar com o serviço de IA.</response>
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
                    return NotFound($"Abrigo com ID {idAbrigo} não encontrado.");
                }

                // 2. Buscar Pessoas (modelo) com check-in ativo no abrigo
                var pessoasNoAbrigo = await _abrigoRepository.GetPessoasAtivasByAbrigoIdAsync(idAbrigo);

                // O GenerativeAIService.GenerateShelterHealthAnalysis já trata o caso de lista de pessoas vazia.
                // if (!pessoasNoAbrigo.Any())
                // {
                //    return Ok(new AnalysisResultDto { Analysis = "Nenhuma pessoa com check-in ativo encontrada no abrigo para análise de saúde." });
                // }
                
                // 3. Chamar o serviço de IA
                var healthAnalysisText = await _generativeAIService.GenerateShelterHealthAnalysis(abrigo, pessoasNoAbrigo);

                var analysisResultDto = new AnalysisResultDto { Analysis = healthAnalysisText };
                AddLinksToHealthAnalysis(analysisResultDto, idAbrigo);

                return Ok(analysisResultDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro em GetShelterHealthAnalysis para o abrigo {idAbrigo}: {ex}");
                // Não retornar NotFound para "Pessoa não encontrada" aqui, pois o serviço de IA deve lidar com lista vazia.
                // Apenas NotFound para o Abrigo.
                return StatusCode(500, $"Erro interno ao gerar análise de saúde: {ex.Message}");
            }
        }
    }
}