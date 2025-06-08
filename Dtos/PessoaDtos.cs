using AbrigueSe.Models; // For Abrigo, Usuario, CheckIn model types if used directly
using System;
using System.ComponentModel.DataAnnotations;

namespace AbrigueSe.Dtos
{
    public class PessoaDto // DTO para criação/atualização de Pessoa
    {
        [Required(ErrorMessage = "O nome da pessoa é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O nome da pessoa deve ter no máximo 100 caracteres.")]
        public string NmPessoa { get; set; } = string.Empty;

        [Required(ErrorMessage = "O CPF é obrigatório.")]
        [StringLength(14, MinimumLength = 11, ErrorMessage = "O CPF deve ter entre 11 e 14 caracteres.")]
        [RegularExpression(@"^\d{3}\.?\d{3}\.?\d{3}-?\d{2}$", ErrorMessage = "Formato de CPF inválido.")]
        public string NrCpf { get; set; } = string.Empty;

        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        public DateTime DtNascimento { get; set; }

        [Required(ErrorMessage = "A condição médica é obrigatória.")]
        public string DsCondicaoMedica { get; set; } = string.Empty;

        [Required(ErrorMessage = "O status de desaparecido é obrigatório.")]
        [RegularExpression("^[SNsn]$", ErrorMessage = "O status de desaparecido deve ser 'S' ou 'N'.")]
        public char StDesaparecido { get; set; }

        [Required(ErrorMessage = "O nome emergencial é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O nome emergencial deve ter no máximo 100 caracteres.")]
        public string NmEmergencial { get; set; } = string.Empty;

        [Required(ErrorMessage = "O contato de emergência é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O contato de emergência deve ter no máximo 100 caracteres.")]
        public string ContatoEmergencia { get; set; } = string.Empty;

        [Required(ErrorMessage = "O ID do endereço é obrigatório.")]
        public int IdEndereco { get; set; }
    }

    public class PessoaGetDto // DTO para retornar informações da Pessoa
    {
        public int IdPessoa { get; set; }
        public string NmPessoa { get; set; } = string.Empty;
        public string NrCpf { get; set; } = string.Empty;
        public DateTime DtNascimento { get; set; }
        public string DsCondicaoMedica { get; set; } = string.Empty;
        public char StDesaparecido { get; set; }
        public string NmEmergencial { get; set; } = string.Empty;
        public string ContatoEmergencia { get; set; } = string.Empty;
        
        public int IdEndereco { get; set; }
        public Endereco? Endereco { get; set; } // Assumes EnderecoGetDto exists
        public Abrigo? AbrigoAtual { get; set; } // Assumes AbrigoGetDto exists
        public Usuario? Usuario { get; set; }   // Assumes UsuarioGetDto exists
        public CheckIn? UltimoCheckIn { get; set; } // Assumes CheckInGetDto exists
    }
}
