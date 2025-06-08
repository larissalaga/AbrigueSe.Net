using OpenAI.Chat;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using AbrigueSe.Models;
using System;
using System.Threading.Tasks;
using AbrigueSe.Dtos; // Mantido caso outros métodos o utilizem
using System.Collections.Generic; // Adicionado para List
using System.Text; // Adicionado para StringBuilder

namespace AbrigueSe.MlModels
{
    public class GenerativeAIService
    {
        private readonly AzureOpenAIClient _client;
        private readonly string _deploymentName;
        private readonly ChatClient _chatClient;

        public GenerativeAIService(IConfiguration configuration)
        {
            var apiKey = configuration["Azure:OpenAI:ApiKey"];
            var endpoint = configuration["Azure:OpenAI:Endpoint"];
            _deploymentName = configuration["Azure:OpenAI:DeploymentName"] ?? "model-router"; // Usar um nome de implantação padrão se não configurado

            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(endpoint))
            {
                // Considerar lançar uma exceção mais específica ou logar o erro.
                throw new ArgumentException("A configuração da API Azure OpenAI está ausente ou incompleta (ApiKey, Endpoint).");
            }

            _client = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
            _chatClient = _client.GetChatClient(_deploymentName);
        }

        // Assinatura do método alterada para receber Abrigo (Model) e List<EstoqueRecurso>
        public async Task<string> GenerateInventoryAnalisys(Abrigo abrigo, List<EstoqueRecurso> estoqueRecursos)
        {
            if (abrigo == null)
            {
                throw new ArgumentNullException(nameof(abrigo), "O objeto Abrigo não pode ser nulo.");
            }
            if (estoqueRecursos == null)
            {
                throw new ArgumentNullException(nameof(estoqueRecursos), "A lista de EstoqueRecurso não pode ser nula.");
            }

            try
            {
                var chatCompletionsOptions = new ChatCompletionOptions()
                {
                    MaxOutputTokenCount = 8192, // Ajustado para um valor mais comum para análises, 8192 pode ser excessivo.
                    Temperature = 0.7f,
                    TopP = 0.95f,
                    // FrequencyPenalty e PresencePenalty mantidos em 0.0f por padrão.
                };

                var systemMessage = "Você é um assistente especializado em análise de recursos para abrigos em tempos de crise. " +
                                    "Com base nos dados fornecidos sobre o estoque de um abrigo e o número de pessoas, " +
                                    "avalie quanto tempo de autonomia o abrigo ainda tem para cada recurso e identifique os recursos mais críticos que precisam ser repostos. " +
                                    "Forneça uma análise concisa e recomendações claras.";

                var userMessageBuilder = new StringBuilder();
                userMessageBuilder.AppendLine($"Análise de Inventário para o Abrigo: {abrigo.NmAbrigo}");
                userMessageBuilder.AppendLine($"Número Atual de Pessoas no Abrigo: {abrigo.NrOcupacaoAtual}");
                userMessageBuilder.AppendLine("\nRecursos em Estoque:");

                if (estoqueRecursos.Any())
                {
                    foreach (var estoque in estoqueRecursos)
                    {
                        if (estoque.Recurso == null) continue; // Pular se o recurso associado for nulo

                        userMessageBuilder.AppendLine($"- Recurso: {estoque.Recurso.DsRecurso}");
                        userMessageBuilder.AppendLine($"  Quantidade Disponível: {estoque.QtDisponivel}");
                        userMessageBuilder.AppendLine($"  Consumo Estimado por Pessoa/Dia: {estoque.Recurso.QtPessoaDia}");
                        userMessageBuilder.AppendLine($"  Consumível: {(estoque.Recurso.StConsumivel == 'S' ? "Sim" : "Não")}");
                        /*
                        // Calcular consumo total diário para o abrigo (se houver pessoas e consumo > 0)
                        if (abrigo.NrOcupacaoAtual > 0 && estoque.Recurso.QtPessoaDia > 0)
                        {
                            double consumoTotalDiario = abrigo.NrOcupacaoAtual * estoque.Recurso.QtPessoaDia;
                            double diasAutonomia = estoque.QtDisponivel / consumoTotalDiario;
                            userMessageBuilder.AppendLine($"  Autonomia Estimada (dias): {diasAutonomia:F1}");
                        }
                        else if (estoque.Recurso.QtPessoaDia == 0 && estoque.Recurso.StConsumivel == 'S')
                        {
                             userMessageBuilder.AppendLine($"  Autonomia Estimada (dias): Indeterminado (consumo por pessoa/dia é zero, mas é consumível)");
                        }
                        else if (estoque.Recurso.StConsumivel != 'S')
                        {
                            userMessageBuilder.AppendLine($"  Autonomia Estimada (dias): Não aplicável (não consumível)");
                        }
                        else
                        {
                             userMessageBuilder.AppendLine($"  Autonomia Estimada (dias): Indeterminado (sem ocupantes ou consumo zero)");
                        }
                        userMessageBuilder.AppendLine($"  Data da Última Atualização do Estoque: {estoque.DtAtualizacao:dd/MM/yyyy}");
                        */
                    }
                }
                else
                {
                    userMessageBuilder.AppendLine("Nenhum recurso em estoque encontrado para este abrigo.");
                }
                
                userMessageBuilder.AppendLine("\nCom base nestes dados, por favor, forneça sua análise e recomendações.");

                List<ChatMessage> messages = new List<ChatMessage>()
                {
                    new SystemChatMessage(systemMessage),
                    new UserChatMessage(userMessageBuilder.ToString())
                };

                ChatCompletion response = await _chatClient.CompleteChatAsync(messages, chatCompletionsOptions);
                
                // Acessar o conteúdo da resposta. A API pode retornar múltiplas escolhas (choices),
                // mas geralmente pegamos a primeira.
                string recommendation = "Não foi possível gerar uma análise no momento."; // Mensagem padrão
                if (response.Content != null && response.Content.Count > 0 && response.Content[0].Text != null)
                {
                    recommendation = response.Content[0].Text.Trim();
                }


                return recommendation;
            }
            catch (RequestFailedException rfEx)
            {
                // Tratar erros específicos da API da Azure OpenAI
                Console.WriteLine($"Erro na requisição à API Azure OpenAI: {rfEx.Status} - {rfEx.Message}");
                // Você pode querer retornar uma mensagem mais amigável ou logar detalhes específicos
                return $"Erro ao comunicar com o serviço de IA: {rfEx.Message}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao gerar análise de inventário: {ex.Message}");
                // Retornar uma mensagem de erro genérica ou específica, dependendo da política de erros
                return "Ocorreu um erro ao processar a análise de inventário. Tente novamente mais tarde.";
            }
        }
    }
}