using AbrigueSe.Models;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; // Para List

namespace AbrigueSe.Dtos
{
    /// <summary>
    /// DTO para criação e atualização de um estado (unidade federativa).
    /// </summary>
    public class EstadoDto : ResourceBaseDto // For Create/Update
    {
        /// <summary>
        /// Nome do estado.
        /// </summary>
        /// <example>Rio de Janeiro</example>
        [Required(ErrorMessage = "O nome do estado é obrigatório.")]
        [MaxLength(50, ErrorMessage = "O nome do estado deve ter no máximo 50 caracteres.")]
        public string NmEstado { get; set; } = string.Empty;

        /// <summary>
        /// Sigla do estado.
        /// </summary>
        /// <example>RJ</example>
        [Required(ErrorMessage = "A sigla do estado é obrigatória.")]
        [MaxLength(2, ErrorMessage = "A sigla do estado deve ter no máximo 2 caracteres.")]
        public string SgEstado { get; set; } = string.Empty;

        /// <summary>
        /// ID do país ao qual o estado pertence.
        /// </summary>
        /// <example>1</example>
        [Required(ErrorMessage = "O ID do país é obrigatório.")]
        public int IdPais { get; set; }
    }

    // EstadoGetDto já foi movido para EnderecoDtos.cs e comentado lá.
    // Se ele pertencesse aqui, os comentários seriam:
    
    /// <summary>
    /// DTO para visualização de dados de um estado.
    /// </summary>
    public class EstadoGetDto : ResourceBaseDto
    {
        /// <summary>
        /// ID único do estado.
        /// </summary>
        /// <example>10</example>
        public int IdEstado { get; set; }
        /// <summary>
        /// Nome do estado.
        /// </summary>
        /// <example>Minas Gerais</example>
        public string NmEstado { get; set; } = string.Empty;
        /// <summary>
        /// Sigla do estado.
        /// </summary>
        /// <example>MG</example>
        public string SgEstado { get; set; } = string.Empty;
        /// <summary>
        /// ID do país ao qual o estado pertence.
        /// </summary>
        /// <example>1</example>
        public int IdPais { get; set; }
        /// <summary>
        /// Detalhes do país associado ao estado.
        /// </summary>
        public Pais? Pais { get; set; }
    }    
}