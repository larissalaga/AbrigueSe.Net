namespace AbrigueSe.Dtos
{
    /// <summary>
    /// DTO para encapsular o resultado de uma an�lise de IA.
    /// </summary>
    public class AnalysisResultDto : ResourceBaseDto
    {
        /// <summary>
        /// O texto da an�lise gerada.
        /// </summary>
        /// <example>Com base no estoque atual, o abrigo tem autonomia de 3 dias para �gua e 2 dias para alimentos n�o perec�veis. � cr�tico repor alimentos enlatados e kits de higiene pessoal.</example>
        public string Analysis { get; set; } = string.Empty;
    }
}