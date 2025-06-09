using AbrigueSe.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace AbrigueSe.Dtos
{
    /// <summary>
    /// DTO para criar ou atualizar um item no estoque de um abrigo.
    /// </summary>
    public class EstoqueRecursoDto : ResourceBaseDto // For Create/Update
    {
        /// <summary>
        /// Quantidade dispon�vel do recurso no estoque.
        /// </summary>
        /// <example>50</example>
        [Required(ErrorMessage = "A quantidade dispon�vel � obrigat�ria.")]
        [Range(0, int.MaxValue, ErrorMessage = "A quantidade dispon�vel n�o pode ser negativa.")]
        public int QtDisponivel { get; set; }

        /// <summary>
        /// ID do abrigo onde o recurso est� estocado.
        /// </summary>
        /// <example>10</example>
        [Required(ErrorMessage = "O ID do abrigo � obrigat�rio.")]
        public int IdAbrigo { get; set; }

        /// <summary>
        /// ID do tipo de recurso estocado.
        /// </summary>
        /// <example>1</example>
        [Required(ErrorMessage = "O ID do recurso � obrigat�rio.")]
        public int IdRecurso { get; set; }
    }

    /// <summary>
    /// DTO para visualiza��o de um item de estoque.
    /// </summary>
    public class EstoqueRecursoGetDto : ResourceBaseDto // For Read
    {
        /// <summary>
        /// ID �nico do registro de estoque.
        /// </summary>
        /// <example>5</example>
        public int IdEstoque { get; set; }

        /// <summary>
        /// Quantidade dispon�vel do recurso no estoque.
        /// </summary>
        /// <example>150</example>
        public int QtDisponivel { get; set; }

        /// <summary>
        /// Data da �ltima atualiza��o do registro de estoque.
        /// </summary>
        /// <example>2023-01-01T00:00:00</example>
        public DateTime DtAtualizacao { get; set; }

        /// <summary>
        /// ID do abrigo onde o recurso est� estocado.
        /// </summary>
        /// <example>10</example>
        public int IdAbrigo { get; set; }

        /// <summary>
        /// Nome do abrigo (para facilitar a visualiza��o).
        /// </summary>
        /// <example>Abrigo Esperan�a</example>
        public Abrigo? Abrigo { get; set; } // Related Abrigo

        /// <summary>
        /// ID do tipo de recurso estocado.
        /// </summary>
        /// <example>1</example>
        public int IdRecurso { get; set; }

        /// <summary>
        /// Descri��o do recurso (para facilitar a visualiza��o).
        /// </summary>
        /// <example>�gua Mineral 500ml</example>
        public Recurso? Recurso { get; set; } // Related Recurso     
    }
}