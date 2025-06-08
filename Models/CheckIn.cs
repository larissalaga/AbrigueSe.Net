using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbrigueSe.Models
{
    [Table("t_gsab_check_in")]
    public class CheckIn
    {
        [Key]
        [Column("id_checkin")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdCheckin { get; set; }

        [Required(ErrorMessage = "A data de entrada é obrigatória.")]
        [Column("dt_entrada", TypeName = "DATE")]
        public DateTime DtEntrada { get; set; }

        [Column("dt_saida", TypeName = "DATE")]
        public DateTime? DtSaida { get; set; } // Nullable as per DDL

        [Required(ErrorMessage = "O ID do abrigo é obrigatório.")]
        [Column("id_abrigo")]
        public int IdAbrigo { get; set; }

        [ForeignKey("IdAbrigo")]
        public virtual Abrigo? Abrigo { get; set; }

        [Required(ErrorMessage = "O ID da pessoa é obrigatório.")]
        [Column("id_pessoa")]
        public int IdPessoa { get; set; }

        [ForeignKey("IdPessoa")]
        public virtual Pessoa? Pessoa { get; set; }
    }
}