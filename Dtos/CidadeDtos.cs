using AbrigueSe.Models;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; // Para List

namespace AbrigueSe.Dtos
{
    /// <summary>
    /// DTO para criação e atualização de uma cidade.
    /// </summary>
    public class CidadeDto : ResourceBaseDto // For Create/Update
    {
        /// <summary>
        /// Nome da cidade.
        /// </summary>
        /// <example>Niterói</example>
        [Required(ErrorMessage = "O nome da cidade é obrigatório.")]
        [MaxLength(80, ErrorMessage = "O nome da cidade deve ter no máximo 80 caracteres.")]
        public string NmCidade { get; set; } = string.Empty;

        /// <summary>
        /// ID do estado ao qual a cidade pertence.
        /// </summary>
        /// <example>19</example> <!-- Exemplo para Rio de Janeiro -->
        [Required(ErrorMessage = "O ID do estado é obrigatório.")]
        public int IdEstado { get; set; }
    }

    // CidadeGetDto já foi movido para EnderecoDtos.cs e comentado lá.
    // Se ele pertencesse aqui, os comentários seriam:
    
    /// <summary>
    /// DTO para visualização de dados de uma cidade.
    /// </summary>
    public class CidadeGetDto : ResourceBaseDto
    {
        /// <summary>
        /// ID único da cidade.
        /// </summary>
        /// <example>301</example>
        public int IdCidade { get; set; }
        /// <summary>
        /// Nome da cidade.
        /// </summary>
        /// <example>Belo Horizonte</example>
        public string NmCidade { get; set; } = string.Empty;
        /// <summary>
        /// ID do estado ao qual a cidade pertence.
        /// </summary>
        /// <example>13</example> <!-- Exemplo para Minas Gerais -->
        public int IdEstado { get; set; }
        /// <summary>
        /// Detalhes do estado associado à cidade.
        /// </summary>
        public Estado? Estado { get; set; }
    }
}