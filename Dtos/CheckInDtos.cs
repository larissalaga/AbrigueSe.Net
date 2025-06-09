using AbrigueSe.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; // Para List

namespace AbrigueSe.Dtos
{
    /// <summary>
    /// DTO para registrar um check-in de uma pessoa em um abrigo.
    /// </summary>
    public class CheckInDto : ResourceBaseDto // For Create/Update
    {
        /// <summary>
        /// A data de entrada é obrigatória.
        /// </summary>
        [Required(ErrorMessage = "A data de entrada é obrigatória.")]
        public DateTime DtEntrada { get; set; } = DateTime.UtcNow; // Default to now for creation

        /// <summary>
        /// Data e hora em que o check-out foi realizado (saída do abrigo), se aplicável.
        /// </summary>
        public DateTime? DtSaida { get; set; } // Nullable for active check-ins

        /// <summary>
        /// ID do abrigo onde o check-in está sendo realizado.
        /// </summary>
        /// <example>10</example>
        [Required(ErrorMessage = "O ID do abrigo é obrigatório.")]
        public int IdAbrigo { get; set; }

        /// <summary>
        /// ID da pessoa que está realizando o check-in.
        /// </summary>
        /// <example>101</example>
        [Required(ErrorMessage = "O ID da pessoa é obrigatório.")]
        public int IdPessoa { get; set; }
    }

    /// <summary>
    /// DTO para visualização de dados de um registro de check-in.
    /// </summary>
    public class CheckInGetDto : ResourceBaseDto // For Read
    {
        /// <summary>
        /// ID único do registro de check-in.
        /// </summary>
        /// <example>55</example>
        public int IdCheckin { get; set; }

        /// <summary>
        /// Data e hora em que o check-in foi realizado (entrada no abrigo).
        /// </summary>
        /// <example>2024-05-20T10:30:00Z</example>
        public DateTime DtEntrada { get; set; }

        /// <summary>
        /// Data e hora em que o check-out foi realizado (saída do abrigo), se aplicável.
        /// </summary>
        /// <example>2024-05-22T15:00:00Z</example>
        public DateTime? DtSaida { get; set; }

        /// <summary>
        /// ID do abrigo onde o check-in foi realizado.
        /// </summary>
        /// <example>10</example>
        public int IdAbrigo { get; set; }

        /// <summary>
        /// Detalhes do abrigo associado ao check-in.
        /// </summary>
        public Abrigo? Abrigo { get; set; } // Related Abrigo

        /// <summary>
        /// ID da pessoa que realizou o check-in.
        /// </summary>
        /// <example>101</example>
        public int IdPessoa { get; set; }

        /// <summary>
        /// Detalhes da pessoa associada ao check-in.
        /// </summary>
        public Pessoa? Pessoa { get; set; } // Related Pessoa        
    }
}