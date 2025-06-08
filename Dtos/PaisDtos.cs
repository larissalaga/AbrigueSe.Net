using AbrigueSe.Models;
using System.ComponentModel.DataAnnotations;

namespace AbrigueSe.Dtos
{
    public class PaisCreateDto
    {
        [Required(ErrorMessage = "O nome do país é obrigatório.")]
        [MaxLength(100)]
        public string NmPais { get; set; }
    }

    public class PaisUpdateDto
    {
        [Required(ErrorMessage = "O nome do país é obrigatório.")]
        [MaxLength(100)]
        public string NmPais { get; set; }
    }

    public class PaisGetDto // For Read
    {
        public int IdPais { get; set; }
        public string NmPais { get; set; } = string.Empty;
        
        public ICollection<Estado>? Estados { get; set; }
    }
}