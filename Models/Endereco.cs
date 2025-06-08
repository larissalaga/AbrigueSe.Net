using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbrigueSe.Models
{
    [Table("t_gsab_endereco")]
    public class Endereco
    {
        [Key]
        [Column("id_endereco")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdEndereco { get; set; }

        [Required(ErrorMessage = "O CEP é obrigatório.")]
        [Column("ds_cep")]
        [MaxLength(11)]
        public string DsCep { get; set; } = string.Empty;

        [Required(ErrorMessage = "O logradouro é obrigatório.")]
        [Column("ds_logradouro")]
        [MaxLength(100)]
        public string DsLogradouro { get; set; } = string.Empty;

        [Required(ErrorMessage = "O número é obrigatório.")]
        [Column("nr_numero")]
        public int NrNumero { get; set; }

        [Required(ErrorMessage = "O complemento é obrigatório.")]
        [Column("ds_complemento")]
        [MaxLength(100)]
        public string DsComplemento { get; set; } = string.Empty;

        [Required(ErrorMessage = "O ID da cidade é obrigatório.")]
        [Column("id_cidade")]
        public int IdCidade { get; set; }

        [ForeignKey("IdCidade")]
        public virtual Cidade? Cidade { get; set; }
    }
}