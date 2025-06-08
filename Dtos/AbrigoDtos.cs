using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AbrigueSe.Models;
using System.Collections.Generic; // For ICollection if needed

namespace AbrigueSe.Dtos
{
    // AbrigoDto (Data Transfer Object for Abrigo)
    [Table("t_gsab_abrigo")] // Nome da tabela conforme DDL
    public class AbrigoDto
    {
        [Key]
        [Column("id_abrigo")] // Nome da coluna conforme DDL
        public int IdAbrigo { get; set; }

        [Required(ErrorMessage = "O nome do abrigo é obrigatório.")]
        [Column("nm_abrigo")] // Nome da coluna conforme DDL
        [MaxLength(100, ErrorMessage = "O nome do abrigo deve ter no máximo 100 caracteres.")]
        public string NmAbrigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "A capacidade do abrigo é obrigatória.")]
        [Column("nr_capacidade")] // Nome da coluna conforme DDL
        [Range(1, int.MaxValue, ErrorMessage = "A capacidade deve ser um número positivo.")]
        public int NrCapacidade { get; set; }

        [Required(ErrorMessage = "A ocupação atual é obrigatória.")]
        [Column("nr_ocupacao_atual")] // Nome da coluna conforme DDL
        [Range(0, int.MaxValue, ErrorMessage = "A ocupação atual não pode ser negativa.")]
        public int NrOcupacaoAtual { get; set; }

        [Required(ErrorMessage = "O ID do endereço é obrigatório.")]
        [Column("id_endereco")]
        public int IdEndereco { get; set; }
    }

    public class AbrigoGetDto
    {
        [Key]
        [Column("id_abrigo")] // Nome da coluna conforme DDL
        public int IdAbrigo { get; set; }
        [Required(ErrorMessage = "O nome do abrigo é obrigatório.")]
        [Column("nm_abrigo")] // Nome da coluna conforme DDL
        [MaxLength(100, ErrorMessage = "O nome do abrigo deve ter no máximo 100 caracteres.")]
        public string NmAbrigo { get; set; } = string.Empty;
        [Required(ErrorMessage = "A capacidade do abrigo é obrigatória.")]
        [Column("nr_capacidade")] // Nome da coluna conforme DDL
        [Range(1, int.MaxValue, ErrorMessage = "A capacidade deve ser um número positivo.")]
        public int NrCapacidade { get; set; }
        [Required(ErrorMessage = "A ocupação atual é obrigatória.")]
        [Column("nr_ocupacao_atual")] // Nome da coluna conforme DDL
        [Range(0, int.MaxValue, ErrorMessage = "A ocupação atual não pode ser negativa.")]
        public int NrOcupacaoAtual { get; set; }

        [Required(ErrorMessage = "O ID do endereço é obrigatório.")]
        [Column("id_endereco")]
        public int IdEndereco { get; set; }

        [ForeignKey("IdEndereco")]
        public virtual Endereco? Endereco { get; set; }
        //public virtual ICollection<CheckIn>? CheckIns { get; set; } // Propriedade de navegação para CheckIn
        public virtual ICollection<EstoqueRecurso>? EstoqueRecursos { get; set; } // Propriedade de navegação para EstoqueRecurso
        public virtual ICollection<Pessoa>? Pessoas { get; set; } // Propriedade de navegação para Pessoa, se necessário
    }
        public class AbrigoCreateDto
    {
        [Required(ErrorMessage = "O nome do abrigo é obrigatório.")]
        [Column("nm_abrigo")] // Nome da coluna conforme DDL
        [MaxLength(100, ErrorMessage = "O nome do abrigo deve ter no máximo 100 caracteres.")]
        public string NmAbrigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "A capacidade do abrigo é obrigatória.")]
        [Column("nr_capacidade")] // Nome da coluna conforme DDL
        [Range(1, int.MaxValue, ErrorMessage = "A capacidade deve ser um número positivo.")]
        public int NrCapacidade { get; set; }

        [Required(ErrorMessage = "A ocupação atual é obrigatória.")]
        [Column("nr_ocupacao_atual")] // Nome da coluna conforme DDL
        [Range(0, int.MaxValue, ErrorMessage = "A ocupação atual não pode ser negativa.")]
        public int NrOcupacaoAtual { get; set; }

        [Required(ErrorMessage = "O ID do endereço é obrigatório.")]
        [Column("id_endereco")]
        public int IdEndereco { get; set; }
    }
}
