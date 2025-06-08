using AbrigueSe.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace AbrigueSe.Dtos
{
    public class CheckInDto // For Create/Update
    {
        [Required(ErrorMessage = "A data de entrada é obrigatória.")]
        public DateTime DtEntrada { get; set; } = DateTime.UtcNow; // Default to now for creation

        public DateTime? DtSaida { get; set; } // Nullable for active check-ins

        [Required(ErrorMessage = "O ID do abrigo é obrigatório.")]
        public int IdAbrigo { get; set; }

        [Required(ErrorMessage = "O ID da pessoa é obrigatório.")]
        public int IdPessoa { get; set; }
    }

    public class CheckInGetDto // For Read
    {
        public int IdCheckin { get; set; }
        public DateTime DtEntrada { get; set; }
        public DateTime? DtSaida { get; set; }
        public int IdAbrigo { get; set; }
        public Abrigo? Abrigo { get; set; } // Related Abrigo
        public int IdPessoa { get; set; }
        public Pessoa? Pessoa { get; set; } // Related Pessoa        
    }
}