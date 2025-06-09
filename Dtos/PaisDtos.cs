using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; // Para List

namespace AbrigueSe.Dtos
{
    /// <summary>
    /// DTO para criação de um novo país.
    /// </summary>
    public class PaisCreateDto
    {
        /// <summary>
        /// Nome do país.
        /// </summary>
        /// <example>Argentina</example>
        [Required(ErrorMessage = "O nome do país é obrigatório.")]
        [MaxLength(50, ErrorMessage = "O nome do país deve ter no máximo 50 caracteres.")]
        public string NmPais { get; set; } = string.Empty;

        /// <summary>
        /// Sigla do país.
        /// </summary>
        /// <example>AR</example>
        [Required(ErrorMessage = "A sigla do país é obrigatória.")]
        [MaxLength(3, ErrorMessage = "A sigla do país deve ter no máximo 3 caracteres.")]
        public string SgPais { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para atualização de um país existente.
    /// </summary>
    public class PaisUpdateDto
    {
        /// <summary>
        /// Novo nome do país (opcional).
        /// </summary>
        /// <example>República Argentina</example>
        [MaxLength(50, ErrorMessage = "O nome do país deve ter no máximo 50 caracteres.")]
        public string? NmPais { get; set; }

        /// <summary>
        /// Nova sigla do país (opcional).
        /// </summary>
        /// <example>ARG</example>
        [MaxLength(3, ErrorMessage = "A sigla do país deve ter no máximo 3 caracteres.")]
        public string? SgPais { get; set; }
    }

    // PaisGetDto já foi movido para EnderecoDtos.cs e comentado lá.
    // Se ele pertencesse aqui, os comentários seriam:
    
    /// <summary>
    /// DTO para visualização de dados de um país.
    /// </summary>
    public class PaisGetDto : ResourceBaseDto
    {
        /// <summary>
        /// ID único do país.
        /// </summary>
        /// <example>2</example>
        public int IdPais { get; set; }
        /// <summary>
        /// Nome do país.
        /// </summary>
        /// <example>Chile</example>
        public string NmPais { get; set; } = string.Empty;
        /// <summary>
        /// Sigla do país.
        /// </summary>
        /// <example>CL</example>
        public string SgPais { get; set; } = string.Empty;
    }
}