using System.Collections.Generic; // Adicionado para ICollection
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbrigueSe.Models
{
    [Table("t_gsab_recurso")] // Nome da tabela conforme DDL
    public class Recurso
    {
        [Key]
        [Column("id_recurso")] // Nome da coluna conforme DDL
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // Manual ID assignment
        public int IdRecurso { get; set; }

        [Required(ErrorMessage = "A descrição do recurso é obrigatória.")] // Adicionada validação e mensagem em PT-BR
        [Column("ds_recurso")] // Nome da coluna conforme DDL
        [MaxLength(100)] // Tamanho máximo
        public string DsRecurso { get; set; } = string.Empty;

        [Required(ErrorMessage = "A quantidade por pessoa/dia é obrigatória.")] // Adicionada validação e mensagem em PT-BR
        [Column("qt_pessoa_dia")] // Nome da coluna conforme DDL
        public int QtPessoaDia { get; set; }

        [Required(ErrorMessage = "O status de consumível é obrigatório.")] // Adicionada validação e mensagem em PT-BR
        [Column("st_consumivel")] // Nome da coluna conforme DDL
        public char StConsumivel { get; set; } // CHAR(1)
    }
}