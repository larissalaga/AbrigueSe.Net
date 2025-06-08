using AbrigueSe.Models;
using System.ComponentModel.DataAnnotations;

namespace AbrigueSe.Dtos
{
    public class PaisDto // For Create/Update
    {
        [Required(ErrorMessage = "O nome do país é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O nome do país deve ter no máximo 100 caracteres.")]
        public string NmPais { get; set; } = string.Empty;
    }

    public class PaisGetDto // For Read
    {
        public int IdPais { get; set; }
        public string NmPais { get; set; } = string.Empty;
        
        public ICollection<Estado>? Estados { get; set; }
    }
}