using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbrigueSe.Models
{
    [Table("t_gsab_usuario")] // Nome da tabela conforme DDL
    public class Usuario
    {
        [Key]
        [Column("id_usuario")] // Nome da coluna conforme DDL
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // Manual ID assignment
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "O nome de usuário é obrigatório.")]
        [Column("nm_usuario")] // Nome da coluna conforme DDL
        [MaxLength(100)]
        public string NmUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório.")]
        [Column("ds_email")] // Nome da coluna conforme DDL
        [MaxLength(100)]
        [EmailAddress]
        public string DsEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [Column("ds_senha")] // Nome da coluna conforme DDL
        [MaxLength(100)] // DDL is 100, ensure this is sufficient for hashed passwords
        public string DsSenha { get; set; } = string.Empty; // A senha deve ser armazenada como hash

        [Required(ErrorMessage = "O código Google é obrigatório.")] // DDL: NOT NULL
        [Column("ds_codigo_google")] // Nome da coluna conforme DDL
        [MaxLength(120)]
        public string DsCodigoGoogle { get; set; } = string.Empty;

        [Required(ErrorMessage = "O ID do tipo de usuário é obrigatório.")]
        [Column("id_tipo_usuario")] // Nome da coluna conforme DDL
        public int IdTipoUsuario { get; set; }
        [ForeignKey("IdTipoUsuario")]
        public virtual TipoUsuario? TipoUsuario { get; set; }

        [Required(ErrorMessage = "O ID da pessoa é obrigatório.")]
        [Column("id_pessoa")] // Nome da coluna conforme DDL
        public int IdPessoa { get; set; } // Esta é uma FK para Pessoa
        [ForeignKey("IdPessoa")]
        public virtual Pessoa? Pessoa { get; set; }
    }
}