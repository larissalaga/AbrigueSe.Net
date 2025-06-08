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
        private readonly GenerativeAIService _generativeAIService; // Injetar o serviço de IA

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
                // O GetById do repositório já lança exceção se não encontrar,
                // então não é necessário checar 'abrigo == null' aqui se essa for a convenção.

                // 2. Buscar o Estoque de Recursos para o Abrigo
                // O método GetByAbrigoId deve incluir os detalhes do Recurso (Recurso.DsRecurso, Recurso.QtPessoaDia)
                var estoqueRecursos = await _estoqueRecursoRepository.GetByAbrigoId(idAbrigo);

                if (abrigo == null) // Dupla checagem caso o repositório não lance exceção
                {
                    return NotFound($"Abrigo com ID {idAbrigo} não encontrado.");
                }

                // 3. Chamar o serviço de IA
                var analysisResult = await _generativeAIService.GenerateInventoryAnalisys(abrigo, estoqueRecursos);

                return Ok(analysisResult);
            }
            catch (Exception ex)
            {
                // Logar a exceção (ex.ToString())
                if (ex.Message.Contains("não encontrado")) // Captura exceções de "não encontrado" dos repositórios
                {
                    return NotFound(ex.Message);
                }
                return StatusCode(500, $"Erro interno ao gerar análise de inventário: {ex.Message}");
            }
        }
    }
}