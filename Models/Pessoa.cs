using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbrigueSe.Models
{
    [Table("t_gsab_pessoa")]
    public class Pessoa
    {
        [Key]
        [Column("id_pessoa")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdPessoa { get; set; }

        [Required(ErrorMessage = "O nome da pessoa é obrigatório.")]
        [Column("nm_pessoa")]
        [MaxLength(100)]
        public string NmPessoa { get; set; } = string.Empty;

        [Required(ErrorMessage = "O CPF é obrigatório.")]
        [Column("nr_cpf")]
        [MaxLength(14)]
        public string NrCpf { get; set; } = string.Empty;

        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        [Column("dt_nascimento", TypeName = "DATE")]
        public DateTime DtNascimento { get; set; }

        [Required(ErrorMessage = "A condição médica é obrigatória.")]
        [Column("ds_condicao_medica")]
        public string DsCondicaoMedica { get; set; } = string.Empty;

        [Required(ErrorMessage = "O status de desaparecido é obrigatório.")]
        [Column("st_desaparecido")]
        public char StDesaparecido { get; set; }

        [Required(ErrorMessage = "O nome emergencial é obrigatório.")]
        [Column("nm_emergencial")]
        [MaxLength(100)]
        public string NmEmergencial { get; set; } = string.Empty;

        [Required(ErrorMessage = "O contato de emergência é obrigatório.")]
        [Column("contato_emergencia")]
        [MaxLength(100)]
        public string ContatoEmergencia { get; set; } = string.Empty;

        [Required(ErrorMessage = "O ID do endereço é obrigatório.")]
        [Column("id_endereco")]
        public int IdEndereco { get; set; }

        [ForeignKey("IdEndereco")]
        public virtual Endereco? Endereco { get; set; }
    }
}
