namespace AbrigueSe.Dtos
{
    /// <summary>
    /// DTO para encapsular o resultado de uma análise de IA.
    /// </summary>
    public class AnalysisResultDto : ResourceBaseDto
    {
        /// <summary>
        /// O texto da análise gerada.
        /// </summary>
        /// <example>Com base no estoque atual, o abrigo tem autonomia de 3 dias para água e 2 dias para alimentos não perecíveis. É crítico repor alimentos enlatados e kits de higiene pessoal.</example>
        public string Analysis { get; set; } = string.Empty;
    }
}