    using AbrigueSe.Models; // For Abrigo, Usuario, CheckIn model types if used directly
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Resources; // Para List

namespace AbrigueSe.Dtos
{
    /// <summary>
    /// DTO para criação e atualização de dados de uma pessoa.
    /// </summary>
    public class PessoaDto : ResourceBaseDto // DTO para criação e atualização de Pessoa
    {
        /// <summary>
        /// Nome completo da pessoa.
        /// </summary>
        /// <example>Carlos Alberto de Nóbrega</example>
        [Required(ErrorMessage = "O nome da pessoa é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        public string NmPessoa { get; set; } = string.Empty;

        /// <summary>
        /// Número do CPF da pessoa (somente dígitos).
        /// </summary>
        /// <example>12345678901</example>
        [Required(ErrorMessage = "O CPF é obrigatório.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "CPF inválido. Deve conter 11 dígitos.")]
        public string NrCpf { get; set; } = string.Empty;

        /// <summary>
        /// Data de nascimento da pessoa.
        /// </summary>
        /// <example>1980-05-15</example>
        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        [DataType(DataType.Date)]
        public DateTime DtNascimento { get; set; }

        /// <summary>
        /// Gênero da pessoa.
        /// </summary>
        /// <example>Masculino</example>
        [Required(ErrorMessage = "O gênero é obrigatório.")]
        [MaxLength(50, ErrorMessage = "O gênero deve ter no máximo 50 caracteres.")]
        public string DsGenero { get; set; } = string.Empty;

        /// <summary>
        /// Número de telefone da pessoa (opcional).
        /// </summary>
        /// <example>(11) 98765-4321</example>
        [MaxLength(20, ErrorMessage = "O telefone deve ter no máximo 20 caracteres.")]
        public string? NrTelefone { get; set; }

        /// <summary>
        /// Endereço de e-mail da pessoa (opcional).
        /// </summary>
        /// <example>carlos.nobrega@example.com</example>
        [MaxLength(100, ErrorMessage = "O e-mail deve ter no máximo 100 caracteres.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        public string? DsEmail { get; set; }

        /// <summary>
        /// ID do endereço residencial da pessoa.
        /// </summary>
        /// <example>45</example>
        [Required(ErrorMessage = "O ID do endereço é obrigatório.")]
        public int IdEndereco { get; set; }

        /// <summary>
        /// ID do abrigo onde a pessoa está atualmente (opcional, se não estiver abrigada).
        /// </summary>
        /// <example>10</example>
        public int? IdAbrigoAtual { get; set; } // Nullable, pois a pessoa pode não estar em um abrigo
    }

    /// <summary>
    /// DTO para visualização de dados de uma pessoa.
    /// </summary>
    public class PessoaGetDto : ResourceBaseDto // DTO para retornar informações da Pessoa
    {
        /// <summary>
        /// ID único da pessoa.
        /// </summary>
        /// <example>201</example>
        public int IdPessoa { get; set; }
        /// <summary>
        /// Nome completo da pessoa.
        /// </summary>
        /// <example>Fernanda Montenegro</example>
        public string NmPessoa { get; set; } = string.Empty;
        /// <summary>
        /// Número do CPF da pessoa (somente dígitos).
        /// </summary>
        /// <example>98765432109</example>
        public string NrCpf { get; set; } = string.Empty; // Considere se o CPF deve ser exposto
        /// <summary>
        /// Data de nascimento da pessoa.
        /// </summary>
        /// <example>1930-10-16</example>
        public DateTime DtNascimento { get; set; }
        /// <summary>
        /// Gênero da pessoa.
        /// </summary>
        /// <example>Feminino</example>
        public string DsGenero { get; set; } = string.Empty;
        /// <summary>
        /// Número de telefone da pessoa.
        /// </summary>
        /// <example>(21) 91234-5678</example>
        public string? NrTelefone { get; set; }
        /// <summary>
        /// Endereço de e-mail da pessoa.
        /// </summary>
        /// <example>fernanda.montenegro@example.com</example>
        public string? DsEmail { get; set; }
        /// <summary>
        /// ID do endereço residencial da pessoa.
        /// </summary>
        /// <example>50</example>
        public int IdEndereco { get; set; }
        /// <summary>
        /// Detalhes do endereço residencial da pessoa.
        /// </summary>
        public Endereco? Endereco { get; set; } // Detalhes do endereço
        /// <summary>
        /// ID do abrigo onde a pessoa está atualmente, se houver.
        /// </summary>
        /// <example>12</example>
        public int? IdAbrigoAtual { get; set; }
        /// <summary>
        /// Detalhes do abrigo atual da pessoa, se houver.
        /// </summary>
        public Abrigo? AbrigoAtual { get; set; } // Detalhes do abrigo atual
        
        /// <summary>
        /// Descrição da condição médica da pessoa. Importante para análise de saúde no abrigo.
        /// </summary>
        /// <example>Hipertensão, Diabetes tipo 2</example>
        public string? DsCondicaoMedica { get; set; } // Adicionado para análise de saúde
        public Usuario? Usuario { get; set; } // Detalhes do usuário associado, se houver
        public CheckIn? UltimoCheckIn { get; set; } // Último CheckIn da pessoa, se houver
    }
}
