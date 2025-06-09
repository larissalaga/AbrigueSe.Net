using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; // Para List

namespace AbrigueSe.Dtos
{
    /// <summary>
    /// DTO para cria��o de um novo pa�s.
    /// </summary>
    public class PaisCreateDto
    {
        /// <summary>
        /// Nome do pa�s.
        /// </summary>
        /// <example>Argentina</example>
        [Required(ErrorMessage = "O nome do pa�s � obrigat�rio.")]
        [MaxLength(50, ErrorMessage = "O nome do pa�s deve ter no m�ximo 50 caracteres.")]
        public string NmPais { get; set; } = string.Empty;

        /// <summary>
        /// Sigla do pa�s.
        /// </summary>
        /// <example>AR</example>
        [Required(ErrorMessage = "A sigla do pa�s � obrigat�ria.")]
        [MaxLength(3, ErrorMessage = "A sigla do pa�s deve ter no m�ximo 3 caracteres.")]
        public string SgPais { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para atualiza��o de um pa�s existente.
    /// </summary>
    public class PaisUpdateDto
    {
        /// <summary>
        /// Novo nome do pa�s (opcional).
        /// </summary>
        /// <example>Rep�blica Argentina</example>
        [MaxLength(50, ErrorMessage = "O nome do pa�s deve ter no m�ximo 50 caracteres.")]
        public string? NmPais { get; set; }

        /// <summary>
        /// Nova sigla do pa�s (opcional).
        /// </summary>
        /// <example>ARG</example>
        [MaxLength(3, ErrorMessage = "A sigla do pa�s deve ter no m�ximo 3 caracteres.")]
        public string? SgPais { get; set; }
    }

    // PaisGetDto j� foi movido para EnderecoDtos.cs e comentado l�.
    // Se ele pertencesse aqui, os coment�rios seriam:
    
    /// <summary>
    /// DTO para visualiza��o de dados de um pa�s.
    /// </summary>
    public class PaisGetDto : ResourceBaseDto
    {
        /// <summary>
        /// ID �nico do pa�s.
        /// </summary>
        /// <example>2</example>
        public int IdPais { get; set; }
        /// <summary>
        /// Nome do pa�s.
        /// </summary>
        /// <example>Chile</example>
        public string NmPais { get; set; } = string.Empty;
        /// <summary>
        /// Sigla do pa�s.
        /// </summary>
        /// <example>CL</example>
        public string SgPais { get; set; } = string.Empty;
    }
}