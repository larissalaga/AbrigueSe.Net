using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbrigueSe.Models
{
    [Table("t_gsab_estado")]
    public class Estado
    {
        [Key]
        [Column("id_estado")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdEstado { get; set; }

        [Required(ErrorMessage = "O nome do estado é obrigatório.")]
        [Column("nm_estado")]
        [MaxLength(100)]
        public string NmEstado { get; set; } = string.Empty;

        [Required(ErrorMessage = "O ID do país é obrigatório.")]
        [Column("id_pais")]
        public int IdPais { get; set; }

        [ForeignKey("IdPais")]
        public virtual Pais? Pais { get; set; }
    }
}