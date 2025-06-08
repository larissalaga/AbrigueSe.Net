using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbrigueSe.Models
{
    [Table("t_gsab_estoque_recurso")] // Nome da tabela conforme DDL
    public class EstoqueRecurso
    {
        [Key]
        [Column("id_estoque")] // Nome da coluna conforme DDL
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // Manual ID assignment
        public int IdEstoque { get; set; }

        [Required(ErrorMessage = "A quantidade disponível é obrigatória.")]
        [Column("qt_disponivel")] // Nome da coluna conforme DDL
        [Range(0, int.MaxValue, ErrorMessage = "A quantidade disponível não pode ser negativa.")]
        public int QtDisponivel { get; set; }

        [Required(ErrorMessage = "A data de atualização é obrigatória.")]
        [Column("dt_atualizacao", TypeName = "DATE")] // Nome da coluna conforme DDL
        public DateTime DtAtualizacao { get; set; }

        [Required(ErrorMessage = "O ID do abrigo é obrigatório.")]
        [Column("id_abrigo")] // Nome da coluna conforme DDL
        public int IdAbrigo { get; set; }
        [ForeignKey("IdAbrigo")]
        public virtual Abrigo? Abrigo { get; set; }

        [Required(ErrorMessage = "O ID do recurso é obrigatório.")]
        [Column("id_recurso")] // Nome da coluna conforme DDL
        public int IdRecurso { get; set; }
        [ForeignKey("IdRecurso")]
        public virtual Recurso? Recurso { get; set; }
    }
}