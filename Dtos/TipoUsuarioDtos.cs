using AbrigueSe.Models;
using System.ComponentModel.DataAnnotations;

namespace AbrigueSe.Dtos
{
    public class TipoUsuarioDto // For Create/Update
    {
        [Required(ErrorMessage = "A descri��o do tipo de usu�rio � obrigat�ria.")]
        [MaxLength(20, ErrorMessage = "A descri��o deve ter no m�ximo 20 caracteres.")]
        public string DsTipoUsuario { get; set; } = string.Empty;
    }

    public class TipoUsuarioGetDto // For Read
    {
        public int IdTipoUsuario { get; set; }
        public string DsTipoUsuario { get; set; } = string.Empty;
        public ICollection<Usuario>? Usuarios { get; set; }
    }
}