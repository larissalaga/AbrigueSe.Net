using AbrigueSe.Models;
using System.ComponentModel.DataAnnotations;

namespace AbrigueSe.Dtos
{
    public class RecursoDto // For Create/Update
    {
        [Required(ErrorMessage = "A descrição do recurso é obrigatória.")]
        [MaxLength(100, ErrorMessage = "A descrição do recurso deve ter no máximo 100 caracteres.")]
        public string DsRecurso { get; set; } = string.Empty;

        [Required(ErrorMessage = "A quantidade por pessoa/dia é obrigatória.")]
        [Range(0, int.MaxValue, ErrorMessage = "A quantidade por pessoa/dia não pode ser negativa.")] // Allow 0 if applicable
        public int QtPessoaDia { get; set; }

        [Required(ErrorMessage = "O status de consumível é obrigatório.")]
        [RegularExpression("^[SNsn]$", ErrorMessage = "O status de consumível deve ser 'S' ou 'N'.")]
        public char StConsumivel { get; set; }
    }

    public class RecursoGetDto // For Read
    {
        public int IdRecurso { get; set; }
        public string DsRecurso { get; set; } = string.Empty;
        public int QtPessoaDia { get; set; }
        public char StConsumivel { get; set; }
        public EstoqueRecurso? UltimoEstoque { get; set; } // Assumes EstoqueRecursoGetDto exists
    }
}

