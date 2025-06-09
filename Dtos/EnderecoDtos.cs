using AbrigueSe.Models;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; // Para List

namespace AbrigueSe.Dtos
{
    /// <summary>
    /// DTO para cria��o e atualiza��o de um endere�o.
    /// </summary>
    public class EnderecoDto : ResourceBaseDto // DTO para cria��o e atualiza��o de Endere�o
    {
        /// <summary>
        /// Logradouro do endere�o (ex: Rua, Avenida).
        /// </summary>
        /// <example>Rua das Palmeiras</example>
        [Required(ErrorMessage = "O logradouro � obrigat�rio.")]
        [MaxLength(100, ErrorMessage = "O logradouro deve ter no m�ximo 100 caracteres.")]
        public string DsLogradouro { get; set; } = string.Empty;

        /// <summary>
        /// N�mero do im�vel no logradouro.
        /// </summary>
        /// <example>123</example>
        [Required(ErrorMessage = "O n�mero � obrigat�rio.")]
        [MaxLength(20, ErrorMessage = "O n�mero deve ter no m�ximo 20 caracteres.")]
        public string NrLogradouro { get; set; } = string.Empty;

        /// <summary>
        /// Complemento do endere�o (ex: Apto 101, Bloco B) (opcional).
        /// </summary>
        /// <example>Casa Fundos</example>
        [MaxLength(50, ErrorMessage = "O complemento deve ter no m�ximo 50 caracteres.")]
        public string? DsComplemento { get; set; }

        /// <summary>
        /// Bairro do endere�o.
        /// </summary>
        /// <example>Vila Madalena</example>
        [Required(ErrorMessage = "O bairro � obrigat�rio.")]
        [MaxLength(50, ErrorMessage = "O bairro deve ter no m�ximo 50 caracteres.")]
        public string DsBairro { get; set; } = string.Empty;

        /// <summary>
        /// CEP do endere�o (somente d�gitos).
        /// </summary>
        /// <example>01234567</example>
        [Required(ErrorMessage = "O CEP � obrigat�rio.")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "CEP inv�lido. Deve conter 8 d�gitos.")]
        public string NrCep { get; set; } = string.Empty;

        /// <summary>
        /// ID da cidade onde o endere�o est� localizado.
        /// </summary>
        /// <example>101</example>
        [Required(ErrorMessage = "O ID da cidade � obrigat�rio.")]
        public int IdCidade { get; set; }
    }

    /// <summary>
    /// DTO para visualiza��o de dados de um endere�o.
    /// </summary>
    public class EnderecoGetDto : ResourceBaseDto // DTO para retornar informa��es do Endere�o
    {
        /// <summary>
        /// ID �nico do endere�o.
        /// </summary>
        /// <example>77</example>
        public int IdEndereco { get; set; }
        /// <summary>
        /// Logradouro do endere�o.
        /// </summary>
        /// <example>Avenida Paulista</example>
        public string DsLogradouro { get; set; } = string.Empty;
        /// <summary>
        /// N�mero do im�vel.
        /// </summary>
        /// <example>1500</example>
        public string NrLogradouro { get; set; } = string.Empty;
        /// <summary>
        /// Complemento do endere�o.
        /// </summary>
        /// <example>Andar 10</example>
        public string? DsComplemento { get; set; }
        /// <summary>
        /// Bairro do endere�o.
        /// </summary>
        /// <example>Bela Vista</example>
        public string DsBairro { get; set; } = string.Empty;
        /// <summary>
        /// CEP do endere�o.
        /// </summary>
        /// <example>01310200</example>
        public string NrCep { get; set; } = string.Empty;
        /// <summary>
        /// ID da cidade do endere�o.
        /// </summary>
        /// <example>102</example>
        public int IdCidade { get; set; }
        /// <summary>
        /// Detalhes da cidade associada ao endere�o.
        /// </summary>
        public Cidade? Cidade { get; set; } // Detalhes da cidade associada
        // Incluir EstadoGetDto e PaisGetDto dentro de CidadeGetDto se necess�rio,
        // ou diretamente aqui se fizer mais sentido para o seu caso de uso.
    }
}