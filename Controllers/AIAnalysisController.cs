using AbrigueSe.Models;
using AbrigueSe.MlModels; // Namespace do GenerativeAIService
using AbrigueSe.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AbrigueSe.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIAnalysisController : ControllerBase
    {
        private readonly IAbrigoRepository _abrigoRepository;
        private readonly IEstoqueRecursoRepository _estoqueRecursoRepository;
        private readonly GenerativeAIService _generativeAIService; // Injetar o servi�o de IA

        public AIAnalysisController(
            IAbrigoRepository abrigoRepository,
            IEstoqueRecursoRepository estoqueRecursoRepository,
            GenerativeAIService generativeAIService)
        {
            _abrigoRepository = abrigoRepository;
            _estoqueRecursoRepository = estoqueRecursoRepository;
            _generativeAIService = generativeAIService;
        }

        // GET: api/AIAnalysis/shelter/{idAbrigo}/inventory-analysis
        [HttpGet("shelter/{idAbrigo}/inventory-analysis")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<string>> GetInventoryAnalysis(int idAbrigo)
        {
            try
            {
                // 1. Buscar o Abrigo
                var abrigo = await _abrigoRepository.GetById(idAbrigo);
                // O GetById do reposit�rio j� lan�a exce��o se n�o encontrar,
                // ent�o n�o � necess�rio checar 'abrigo == null' aqui se essa for a conven��o.

                // 2. Buscar o Estoque de Recursos para o Abrigo
                // O m�todo GetByAbrigoId deve incluir os detalhes do Recurso (Recurso.DsRecurso, Recurso.QtPessoaDia)
                var estoqueRecursos = await _estoqueRecursoRepository.GetByAbrigoId(idAbrigo);

                if (abrigo == null) // Dupla checagem caso o reposit�rio n�o lance exce��o
                {
                    return NotFound($"Abrigo com ID {idAbrigo} n�o encontrado.");
                }

                // 3. Chamar o servi�o de IA
                var analysisResult = await _generativeAIService.GenerateInventoryAnalisys(abrigo, estoqueRecursos);

                return Ok(analysisResult);
            }
            catch (Exception ex)
            {
                // Logar a exce��o (ex.ToString())
                if (ex.Message.Contains("n�o encontrado")) // Captura exce��es de "n�o encontrado" dos reposit�rios
                {
                    return NotFound(ex.Message);
                }
                return StatusCode(500, $"Erro interno ao gerar an�lise de invent�rio: {ex.Message}");
            }
        }
    }
}