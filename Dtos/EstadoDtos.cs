using AbrigueSe.Models;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; // Para List

namespace AbrigueSe.Dtos
{
    /// <summary>
    /// DTO para cria��o e atualiza��o de um estado (unidade federativa).
    /// </summary>
    public class EstadoDto : ResourceBaseDto // For Create/Update
    {
        /// <summary>
        /// Nome do estado.
        /// </summary>
        /// <example>Rio de Janeiro</example>
        [Required(ErrorMessage = "O nome do estado � obrigat�rio.")]
        [MaxLength(50, ErrorMessage = "O nome do estado deve ter no m�ximo 50 caracteres.")]
        public string NmEstado { get; set; } = string.Empty;

        /// <summary>
        /// Sigla do estado.
        /// </summary>
        /// <example>RJ</example>
        [Required(ErrorMessage = "A sigla do estado � obrigat�ria.")]
        [MaxLength(2, ErrorMessage = "A sigla do estado deve ter no m�ximo 2 caracteres.")]
        public string SgEstado { get; set; } = string.Empty;

        /// <summary>
        /// ID do pa�s ao qual o estado pertence.
        /// </summary>
        /// <example>1</example>
        [Required(ErrorMessage = "O ID do pa�s � obrigat�rio.")]
        public int IdPais { get; set; }
    }

    // EstadoGetDto j� foi movido para EnderecoDtos.cs e comentado l�.
    // Se ele pertencesse aqui, os coment�rios seriam:
    
    /// <summary>
    /// DTO para visualiza��o de dados de um estado.
    /// </summary>
    public class EstadoGetDto : ResourceBaseDto
    {
        /// <summary>
        /// ID �nico do estado.
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
        /// ID do pa�s ao qual o estado pertence.
        /// </summary>
        /// <example>1</example>
        public int IdPais { get; set; }
        /// <summary>
        /// Detalhes do pa�s associado ao estado.
        /// </summary>
        public Pais? Pais { get; set; }
    }    
}