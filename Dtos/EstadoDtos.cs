using AbrigueSe.Models;
using System.ComponentModel.DataAnnotations;

namespace AbrigueSe.Dtos
{
    public class EstadoDto // For Create/Update
    {
        [Required(ErrorMessage = "O nome do estado é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O nome do estado deve ter no máximo 100 caracteres.")]
        public string NmEstado { get; set; } = string.Empty;

        [Required(ErrorMessage = "O ID do país é obrigatório.")]
        public int IdPais { get; set; }
    }

    public class EstadoGetDto // For Read
    {
        public int IdEstado { get; set; }
        public string NmEstado { get; set; } = string.Empty;
        public int IdPais { get; set; }
        public Pais? Pais { get; set; } // Related Pais
        public ICollection<Cidade> Cidades { get; set; }
    }
}