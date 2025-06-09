using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AbrigueSe.Models;
using System.Collections.Generic; // For ICollection if needed

namespace AbrigueSe.Dtos
{
    // AbrigoDto (Data Transfer Object for Abrigo)
    [Table("t_gsab_abrigo")] // Nome da tabela conforme DDL
    public class AbrigoDto : ResourceBaseDto
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

    /// <summary>
    /// DTO para visualização de dados de um abrigo.
    /// </summary>
    public class AbrigoGetDto : ResourceBaseDto // DTO para retornar informações do Abrigo
    {
        /// <summary>
        /// ID único do abrigo.
        /// </summary>
        /// <example>10</example>
        public int IdAbrigo { get; set; }
        /// <summary>
        /// Nome do abrigo.
        /// </summary>
        /// <example>Abrigo da Paz</example>
        public string NmAbrigo { get; set; } = string.Empty;
        /// <summary>
        /// Capacidade máxima de pessoas do abrigo.
        /// </summary>
        /// <example>150</example>
        public int NrCapacidade { get; set; }
        /// <summary>
        /// Número atual de ocupantes no abrigo.
        /// </summary>
        /// <example>125</example>
        public int NrOcupacaoAtual { get; set; }
        /// <summary>
        /// ID do endereço do abrigo.
        /// </summary>
        /// <example>30</example>
        public int IdEndereco { get; set; }
        /// <summary>
        /// Detalhes do endereço associado ao abrigo.
        /// </summary>
        public Endereco? Endereco { get; set; } // Detalhes do endereço associado
        /// <summary>
        /// Lista de pessoas atualmente abrigadas (check-in ativo).
        /// </summary>
        public List<Pessoa>? Pessoas { get; set; } // Lista de pessoas no abrigo
        /// <summary>
        /// Lista de recursos e suas quantidades em estoque no abrigo.
        /// </summary>
        public List<EstoqueRecurso>? EstoqueRecursos { get; set; } // Lista de estoque de recursos
    }

    /// <summary>
    /// DTO para criação de um novo abrigo.
    /// </summary>
    public class AbrigoCreateDto // DTO para criação de abrigo
    {
        /// <summary>
        /// Nome do abrigo.
        /// </summary>
        /// <example>Abrigo Esperança</example>
        [Required(ErrorMessage = "O nome do abrigo é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O nome do abrigo deve ter no máximo 100 caracteres.")]
        public string NmAbrigo { get; set; } = string.Empty;

        /// <summary>
        /// Capacidade máxima de pessoas que o abrigo pode comportar.
        /// </summary>
        /// <example>100</example>
        [Required(ErrorMessage = "A capacidade do abrigo é obrigatória.")]
        [Range(1, int.MaxValue, ErrorMessage = "A capacidade deve ser um número positivo.")]
        public int NrCapacidade { get; set; }

        /// <summary>
        /// Número atual de pessoas ocupando o abrigo.
        /// </summary>
        /// <example>75</example>
        [Required(ErrorMessage = "A ocupação atual é obrigatória.")]
        [Range(0, int.MaxValue, ErrorMessage = "A ocupação atual não pode ser negativa.")]
        public int NrOcupacaoAtual { get; set; }

        /// <summary>
        /// ID do endereço onde o abrigo está localizado.
        /// </summary>
        /// <example>25</example>
        [Required(ErrorMessage = "O ID do endereço é obrigatório.")]
        public int IdEndereco { get; set; }
    }

    /// <summary>
    /// DTO para atualização de um abrigo existente.
    /// </summary>
    public class AbrigoUpdateDto // DTO para atualização de abrigo
    {
        /// <summary>
        /// Novo nome do abrigo (opcional).
        /// </summary>
        /// <example>Abrigo Nova Esperança</example>
        [MaxLength(100, ErrorMessage = "O nome do abrigo deve ter no máximo 100 caracteres.")]
        public string? NmAbrigo { get; set; }

        /// <summary>
        /// Nova capacidade máxima do abrigo (opcional).
        /// </summary>
        /// <example>120</example>
        [Range(1, int.MaxValue, ErrorMessage = "A capacidade deve ser um número positivo.")]
        public int? NrCapacidade { get; set; }

        /// <summary>
        /// Nova ocupação atual do abrigo (opcional).
        /// </summary>
        /// <example>80</example>
        [Range(0, int.MaxValue, ErrorMessage = "A ocupação atual não pode ser negativa.")]
        public int? NrOcupacaoAtual { get; set; }

        /// <summary>
        /// Novo ID do endereço do abrigo (opcional).
        /// </summary>
        /// <example>26</example>
        public int? IdEndereco { get; set; }
    }
}
