using AbrigueSe.Models;
using System.ComponentModel.DataAnnotations;

namespace AbrigueSe.Dtos
{
    public class CidadeDto // For Create/Update
    {
        [Required(ErrorMessage = "O nome da cidade � obrigat�rio.")]
        [MaxLength(100, ErrorMessage = "O nome da cidade deve ter no m�ximo 100 caracteres.")]
        public string NmCidade { get; set; } = string.Empty;

        [Required(ErrorMessage = "O ID do estado � obrigat�rio.")]
        public int IdEstado { get; set; }
    }

    public class CidadeGetDto // For Read
    {
        public int IdCidade { get; set; }
        public string NmCidade { get; set; } = string.Empty;
        public int IdEstado { get; set; }
        public Estado? Estado { get; set; } // Related Estado
        public ICollection<Endereco>? Enderecos { get; set; }
    }
}