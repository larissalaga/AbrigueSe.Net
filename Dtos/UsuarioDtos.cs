using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using AbrigueSe.Models; // Necessário para List se ResourceBaseDto não for no mesmo namespace

namespace AbrigueSe.Dtos
{
    /// <summary>
    /// DTO para criação de um novo usuário.
    /// </summary>
    public class UsuarioCreateDto // DTO para criação de usuário
    {
        /// <summary>
        /// Nome do usuário.
        /// </summary>
        /// <example>João Silva</example>
        [Required(ErrorMessage = "O nome de usuário é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O nome de usuário deve ter no máximo 100 caracteres.")]
        public string NmUsuario { get; set; } = string.Empty;

        /// <summary>
        /// Endereço de e-mail do usuário. Deve ser único.
        /// </summary>
        /// <example>joao.silva@example.com</example>
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O e-mail deve ter no máximo 100 caracteres.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        public string DsEmail { get; set; } = string.Empty;

        /// <summary>
        /// Senha do usuário.
        /// </summary>
        /// <example>Senha@123</example>
        [Required(ErrorMessage = "A senha é obrigatória.")]
        [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")] // Exemplo de validação de senha
        [MaxLength(100, ErrorMessage = "A senha deve ter no máximo 100 caracteres.")]
        public string DsSenha { get; set; } = string.Empty;

        /// <summary>
        /// Código de identificação do Google (ex: sub do token ID).
        /// </summary>
        /// <example>109876543210987654321</example>
        [Required(ErrorMessage = "O código Google é obrigatório.")]
        [MaxLength(120, ErrorMessage = "O código Google deve ter no máximo 120 caracteres.")]
        public string DsCodigoGoogle { get; set; } = string.Empty; // Ou o que quer que isso represente

        /// <summary>
        /// ID do tipo de usuário.
        /// </summary>
        /// <example>1</example>
        [Required(ErrorMessage = "O ID do tipo de usuário é obrigatório.")]
        public int IdTipoUsuario { get; set; }

        /// <summary>
        /// ID da pessoa associada a este usuário.
        /// </summary>
        /// <example>101</example>
        [Required(ErrorMessage = "O ID da pessoa é obrigatório.")]
        public int IdPessoa { get; set; } // ID da Pessoa associada
    }

    /// <summary>
    /// DTO para atualização de um usuário existente.
    /// </summary>
    public class UsuarioUpdateDto // DTO para atualização de usuário
    {
        /// <summary>
        /// Novo nome do usuário (opcional).
        /// </summary>
        /// <example>José Almeida</example>
        [MaxLength(100, ErrorMessage = "O nome de usuário deve ter no máximo 100 caracteres.")]
        public string? NmUsuario { get; set; }

        /// <summary>
        /// Novo endereço de e-mail do usuário (opcional). Deve ser único.
        /// </summary>
        /// <example>jose.almeida@example.com</example>
        [MaxLength(100, ErrorMessage = "O e-mail deve ter no máximo 100 caracteres.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        public string? DsEmail { get; set; }

        /// <summary>
        /// Nova senha do usuário (opcional).
        /// </summary>
        /// <example>NovaSenha@456</example>
        [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
        public string? DsSenha { get; set; } // Nullable

        /// <summary>
        /// Novo código de identificação do Google (opcional).
        /// </summary>
        /// <example>210987654321098765432</example>
        [MaxLength(120, ErrorMessage = "O código Google deve ter no máximo 120 caracteres.")]
        public string? DsCodigoGoogle { get; set; }

        /// <summary>
        /// Novo ID do tipo de usuário (opcional).
        /// </summary>
        /// <example>2</example>
        public int? IdTipoUsuario { get; set; }
        // IdPessoa geralmente não é alterado após a criação do usuário
    }

    /// <summary>
    /// DTO para visualização de dados de um usuário.
    /// </summary>
    public class UsuarioGetDto : ResourceBaseDto // DTO para retornar informações do Usuário
    {
        /// <summary>
        /// ID único do usuário.
        /// </summary>
        /// <example>15</example>
        public int IdUsuario { get; set; }
        /// <summary>
        /// Nome do usuário.
        /// </summary>
        /// <example>Maria Oliveira</example>
        public string NmUsuario { get; set; } = string.Empty;
        /// <summary>
        /// Endereço de e-mail do usuário.
        /// </summary>
        /// <example>maria.oliveira@example.com</example>
        public string DsEmail { get; set; } = string.Empty;
        // Não retorne a senha ou código do Google por segurança
        /// <summary>
        /// ID do tipo de usuário.
        /// </summary>
        /// <example>1</example>
        public int IdTipoUsuario { get; set; }
        /// <summary>
        /// Detalhes do tipo de usuário.
        /// </summary>
        public TipoUsuario? TipoUsuario { get; set; } // Related TipoUsuario
        /// <summary>
        /// ID da pessoa associada a este usuário.
        /// </summary>
        /// <example>102</example>
        public int IdPessoa { get; set; }
        /// <summary>
        /// Detalhes da pessoa associada.
        /// </summary>
        public Pessoa? Pessoa { get; set; } // Detalhes da pessoa associada
    }

    /// <summary>
    /// DTO para o processo de login.
    /// </summary>
    public class LoginDto // DTO para login
    {
        /// <summary>
        /// Endereço de e-mail para login.
        /// </summary>
        /// <example>admin@example.com</example>
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        public string DsEmail { get; set; } = string.Empty;

        /// <summary>
        /// Senha para login.
        /// </summary>
        /// <example>AdminSenha123</example>
        [Required(ErrorMessage = "A senha é obrigatória.")]
        public string DsSenha { get; set; } = string.Empty;
    }
}