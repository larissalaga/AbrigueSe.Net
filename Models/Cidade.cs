using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbrigueSe.Models
{
    [Table("t_gsab_cidade")]
    public class Cidade
    {
        [Key]
        [Column("id_cidade")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdCidade { get; set; }

        [Required(ErrorMessage = "O nome da cidade é obrigatório.")]
        [Column("nm_cidade")]
        [MaxLength(100)]
        public string NmCidade { get; set; } = string.Empty;

        [Required(ErrorMessage = "O ID do estado é obrigatório.")]
        [Column("id_estado")]
        public int IdEstado { get; set; }

        [ForeignKey("IdEstado")]
        public virtual Estado? Estado { get; set; }
    }
}