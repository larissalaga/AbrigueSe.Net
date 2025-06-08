using AbrigueSe.Models;
using System.ComponentModel.DataAnnotations;

namespace AbrigueSe.Dtos
{
    public class TipoUsuarioDto // For Create/Update
    {
        [Required(ErrorMessage = "A descrição do tipo de usuário é obrigatória.")]
        [MaxLength(20, ErrorMessage = "A descrição deve ter no máximo 20 caracteres.")]
        public string DsTipoUsuario { get; set; } = string.Empty;
    }

    public class TipoUsuarioGetDto // For Read
    {
        public int IdTipoUsuario { get; set; }
        public string DsTipoUsuario { get; set; } = string.Empty;
        public ICollection<Usuario>? Usuarios { get; set; }
    }
}