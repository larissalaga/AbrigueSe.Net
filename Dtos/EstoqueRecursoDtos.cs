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
        /// Quantidade disponível do recurso no estoque.
        /// </summary>
        /// <example>50</example>
        [Required(ErrorMessage = "A quantidade disponível é obrigatória.")]
        [Range(0, int.MaxValue, ErrorMessage = "A quantidade disponível não pode ser negativa.")]
        public int QtDisponivel { get; set; }

        /// <summary>
        /// ID do abrigo onde o recurso está estocado.
        /// </summary>
        /// <example>10</example>
        [Required(ErrorMessage = "O ID do abrigo é obrigatório.")]
        public int IdAbrigo { get; set; }

        /// <summary>
        /// ID do tipo de recurso estocado.
        /// </summary>
        /// <example>1</example>
        [Required(ErrorMessage = "O ID do recurso é obrigatório.")]
        public int IdRecurso { get; set; }
    }

    /// <summary>
    /// DTO para visualização de um item de estoque.
    /// </summary>
    public class EstoqueRecursoGetDto : ResourceBaseDto // For Read
    {
        /// <summary>
        /// ID único do registro de estoque.
        /// </summary>
        /// <example>5</example>
        public int IdEstoque { get; set; }

        /// <summary>
        /// Quantidade disponível do recurso no estoque.
        /// </summary>
        /// <example>150</example>
        public int QtDisponivel { get; set; }

        /// <summary>
        /// Data da última atualização do registro de estoque.
        /// </summary>
        /// <example>2023-01-01T00:00:00</example>
        public DateTime DtAtualizacao { get; set; }

        /// <summary>
        /// ID do abrigo onde o recurso está estocado.
        /// </summary>
        /// <example>10</example>
        public int IdAbrigo { get; set; }

        /// <summary>
        /// Nome do abrigo (para facilitar a visualização).
        /// </summary>
        /// <example>Abrigo Esperança</example>
        public Abrigo? Abrigo { get; set; } // Related Abrigo

        /// <summary>
        /// ID do tipo de recurso estocado.
        /// </summary>
        /// <example>1</example>
        public int IdRecurso { get; set; }

        /// <summary>
        /// Descrição do recurso (para facilitar a visualização).
        /// </summary>
        /// <example>Água Mineral 500ml</example>
        public Recurso? Recurso { get; set; } // Related Recurso     
    }
}