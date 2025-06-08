using AbrigueSe.Models;
using System.ComponentModel.DataAnnotations;

namespace AbrigueSe.Dtos
{
    public class EnderecoDto // For Create/Update - Assumed correct from Iteration 16
    {
        [Required(ErrorMessage = "O CEP é obrigatório.")]
        [StringLength(11, MinimumLength = 8, ErrorMessage = "O CEP deve ter entre 8 e 11 caracteres.")]
        public string DsCep { get; set; } = string.Empty;

        [Required(ErrorMessage = "O logradouro é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O logradouro deve ter no máximo 100 caracteres.")]
        public string DsLogradouro { get; set; } = string.Empty;

        [Required(ErrorMessage = "O número é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "Número inválido.")]
        public int NrNumero { get; set; }

        [Required(ErrorMessage = "O complemento é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O complemento deve ter no máximo 100 caracteres.")]
        public string DsComplemento { get; set; } = string.Empty;

        [Required(ErrorMessage = "O ID da cidade é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "ID da cidade inválido.")]
        public int IdCidade { get; set; }
        public Cidade? Cidade { get; set; }
    }

    public class EnderecoGetDto // For Read - Using denormalized structure from Iteration 16
    {
        public int IdEndereco { get; set; }
        public string DsCep { get; set; } = string.Empty;
        public string DsLogradouro { get; set; } = string.Empty;
        public int NrNumero { get; set; }
        public string DsComplemento { get; set; } = string.Empty;
        public int IdCidade { get; set; }
        public CidadeGetDto? Cidade { get; set; } 
        public Abrigo? Abrigo { get; set; } // Assuming Abrigo is related to Endereco, if needed
        public Pessoa? Pessoa { get; set; } // Assuming Pessoa is related to Endereco, if needed
    }
}