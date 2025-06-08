using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbrigueSe.Models
{
    [Table("t_gsab_pais")]
    public class Pais
    {
        [Key]
        [Column("id_pais")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdPais { get; set; }

        [Required(ErrorMessage = "O nome do país é obrigatório.")]
        [Column("nm_pais")]
        [MaxLength(100)]
        public string NmPais { get; set; } = string.Empty;
    }
}