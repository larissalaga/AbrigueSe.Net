using AbrigueSe.Models;
using System.ComponentModel.DataAnnotations;

namespace AbrigueSe.Dtos
{
    public class EstadoDto // For Create/Update
    {
        [Required(ErrorMessage = "O nome do estado � obrigat�rio.")]
        [MaxLength(100, ErrorMessage = "O nome do estado deve ter no m�ximo 100 caracteres.")]
        public string NmEstado { get; set; } = string.Empty;

        [Required(ErrorMessage = "O ID do pa�s � obrigat�rio.")]
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