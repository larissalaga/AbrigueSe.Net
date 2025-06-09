using AbrigueSe.Models;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; // Para List

namespace AbrigueSe.Dtos
{
    /// <summary>
    /// DTO para cria��o e atualiza��o de uma cidade.
    /// </summary>
    public class CidadeDto : ResourceBaseDto // For Create/Update
    {
        /// <summary>
        /// Nome da cidade.
        /// </summary>
        /// <example>Niter�i</example>
        [Required(ErrorMessage = "O nome da cidade � obrigat�rio.")]
        [MaxLength(80, ErrorMessage = "O nome da cidade deve ter no m�ximo 80 caracteres.")]
        public string NmCidade { get; set; } = string.Empty;

        /// <summary>
        /// ID do estado ao qual a cidade pertence.
        /// </summary>
        /// <example>19</example> <!-- Exemplo para Rio de Janeiro -->
        [Required(ErrorMessage = "O ID do estado � obrigat�rio.")]
        public int IdEstado { get; set; }
    }

    // CidadeGetDto j� foi movido para EnderecoDtos.cs e comentado l�.
    // Se ele pertencesse aqui, os coment�rios seriam:
    
    /// <summary>
    /// DTO para visualiza��o de dados de uma cidade.
    /// </summary>
    public class CidadeGetDto : ResourceBaseDto
    {
        /// <summary>
        /// ID �nico da cidade.
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
        /// Detalhes do estado associado � cidade.
        /// </summary>
        public Estado? Estado { get; set; }
    }
}