using AbrigueSe.Models;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; // Para List

namespace AbrigueSe.Dtos
{
    /// <summary>
    /// DTO para criação e atualização de um endereço.
    /// </summary>
    public class EnderecoDto : ResourceBaseDto // DTO para criação e atualização de Endereço
    {
        /// <summary>
        /// Logradouro do endereço (ex: Rua, Avenida).
        /// </summary>
        /// <example>Rua das Palmeiras</example>
        [Required(ErrorMessage = "O logradouro é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O logradouro deve ter no máximo 100 caracteres.")]
        public string DsLogradouro { get; set; } = string.Empty;

        /// <summary>
        /// Número do imóvel no logradouro.
        /// </summary>
        /// <example>123</example>
        [Required(ErrorMessage = "O número é obrigatório.")]
        [MaxLength(20, ErrorMessage = "O número deve ter no máximo 20 caracteres.")]
        public string NrLogradouro { get; set; } = string.Empty;

        /// <summary>
        /// Complemento do endereço (ex: Apto 101, Bloco B) (opcional).
        /// </summary>
        /// <example>Casa Fundos</example>
        [MaxLength(50, ErrorMessage = "O complemento deve ter no máximo 50 caracteres.")]
        public string? DsComplemento { get; set; }

        /// <summary>
        /// Bairro do endereço.
        /// </summary>
        /// <example>Vila Madalena</example>
        [Required(ErrorMessage = "O bairro é obrigatório.")]
        [MaxLength(50, ErrorMessage = "O bairro deve ter no máximo 50 caracteres.")]
        public string DsBairro { get; set; } = string.Empty;

        /// <summary>
        /// CEP do endereço (somente dígitos).
        /// </summary>
        /// <example>01234567</example>
        [Required(ErrorMessage = "O CEP é obrigatório.")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "CEP inválido. Deve conter 8 dígitos.")]
        public string NrCep { get; set; } = string.Empty;

        /// <summary>
        /// ID da cidade onde o endereço está localizado.
        /// </summary>
        /// <example>101</example>
        [Required(ErrorMessage = "O ID da cidade é obrigatório.")]
        public int IdCidade { get; set; }
    }

    /// <summary>
    /// DTO para visualização de dados de um endereço.
    /// </summary>
    public class EnderecoGetDto : ResourceBaseDto // DTO para retornar informações do Endereço
    {
        /// <summary>
        /// ID único do endereço.
        /// </summary>
        /// <example>77</example>
        public int IdEndereco { get; set; }
        /// <summary>
        /// Logradouro do endereço.
        /// </summary>
        /// <example>Avenida Paulista</example>
        public string DsLogradouro { get; set; } = string.Empty;
        /// <summary>
        /// Número do imóvel.
        /// </summary>
        /// <example>1500</example>
        public string NrLogradouro { get; set; } = string.Empty;
        /// <summary>
        /// Complemento do endereço.
        /// </summary>
        /// <example>Andar 10</example>
        public string? DsComplemento { get; set; }
        /// <summary>
        /// Bairro do endereço.
        /// </summary>
        /// <example>Bela Vista</example>
        public string DsBairro { get; set; } = string.Empty;
        /// <summary>
        /// CEP do endereço.
        /// </summary>
        /// <example>01310200</example>
        public string NrCep { get; set; } = string.Empty;
        /// <summary>
        /// ID da cidade do endereço.
        /// </summary>
        /// <example>102</example>
        public int IdCidade { get; set; }
        /// <summary>
        /// Detalhes da cidade associada ao endereço.
        /// </summary>
        public Cidade? Cidade { get; set; } // Detalhes da cidade associada
        // Incluir EstadoGetDto e PaisGetDto dentro de CidadeGetDto se necessário,
        // ou diretamente aqui se fizer mais sentido para o seu caso de uso.
    }
}