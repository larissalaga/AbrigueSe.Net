using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbrigueSe.Models
{
    [Table("t_gsab_abrigo")]
    public class Abrigo
    {
        [Key]
        [Column("id_abrigo")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdAbrigo { get; set; }

        [Required(ErrorMessage = "O nome do abrigo � obrigat�rio.")]
        [Column("nm_abrigo")]
        [MaxLength(100)]
        public string NmAbrigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "A capacidade do abrigo � obrigat�ria.")]
        [Column("nr_capacidade")]
        public int NrCapacidade { get; set; }

        [Required(ErrorMessage = "A ocupa��o atual � obrigat�ria.")]
        [Column("nr_ocupacao_atual")]
        public int NrOcupacaoAtual { get; set; }

        [Required(ErrorMessage = "O ID do endere�o � obrigat�rio.")]
        [Column("id_endereco")]
        public int IdEndereco { get; set; }

        [ForeignKey("IdEndereco")]
        public virtual Endereco? Endereco { get; set; }
    }
}