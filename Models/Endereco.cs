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

        [Required(ErrorMessage = "O CEP � obrigat�rio.")]
        [Column("ds_cep")]
        [MaxLength(11)]
        public string DsCep { get; set; } = string.Empty;

        [Required(ErrorMessage = "O logradouro � obrigat�rio.")]
        [Column("ds_logradouro")]
        [MaxLength(100)]
        public string DsLogradouro { get; set; } = string.Empty;

        [Required(ErrorMessage = "O n�mero � obrigat�rio.")]
        [Column("nr_numero")]
        public int NrNumero { get; set; }

        [Required(ErrorMessage = "O complemento � obrigat�rio.")]
        [Column("ds_complemento")]
        [MaxLength(100)]
        public string DsComplemento { get; set; } = string.Empty;

        [Required(ErrorMessage = "O ID da cidade � obrigat�rio.")]
        [Column("id_cidade")]
        public int IdCidade { get; set; }

        [ForeignKey("IdCidade")]
        public virtual Cidade? Cidade { get; set; }
    }
}