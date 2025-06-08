using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbrigueSe.Models
{
    [Table("t_gsab_tipo_usuario")] // Nome da tabela conforme DDL
    public class TipoUsuario
    {
        [Key]
        [Column("id_tipo_usuario")] // Nome da coluna conforme DDL
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // Manual ID assignment
        public int IdTipoUsuario { get; set; }

        [Required(ErrorMessage = "A descri��o do tipo de usu�rio � obrigat�ria.")]
        [Column("ds_tipo_usuario")] // Nome da coluna conforme DDL
        [MaxLength(20, ErrorMessage = "A descri��o do tipo de usu�rio deve ter no m�ximo 20 caracteres.")] // Tamanho m�ximo conforme DDL
        public string DsTipoUsuario { get; set; } = string.Empty;
    }
}