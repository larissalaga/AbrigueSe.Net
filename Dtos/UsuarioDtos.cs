using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using AbrigueSe.Models; // Necess�rio para List se ResourceBaseDto n�o for no mesmo namespace

namespace AbrigueSe.Dtos
{
    /// <summary>
    /// DTO para cria��o de um novo usu�rio.
    /// </summary>
    public class UsuarioCreateDto // DTO para cria��o de usu�rio
    {
        /// <summary>
        /// Nome do usu�rio.
        /// </summary>
        /// <example>Jo�o Silva</example>
        [Required(ErrorMessage = "O nome de usu�rio � obrigat�rio.")]
        [MaxLength(100, ErrorMessage = "O nome de usu�rio deve ter no m�ximo 100 caracteres.")]
        public string NmUsuario { get; set; } = string.Empty;

        /// <summary>
        /// Endere�o de e-mail do usu�rio. Deve ser �nico.
        /// </summary>
        /// <example>joao.silva@example.com</example>
        [Required(ErrorMessage = "O e-mail � obrigat�rio.")]
        [MaxLength(100, ErrorMessage = "O e-mail deve ter no m�ximo 100 caracteres.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inv�lido.")]
        public string DsEmail { get; set; } = string.Empty;

        /// <summary>
        /// Senha do usu�rio.
        /// </summary>
        /// <example>Senha@123</example>
        [Required(ErrorMessage = "A senha � obrigat�ria.")]
        [MinLength(6, ErrorMessage = "A senha deve ter no m�nimo 6 caracteres.")] // Exemplo de valida��o de senha
        [MaxLength(100, ErrorMessage = "A senha deve ter no m�ximo 100 caracteres.")]
        public string DsSenha { get; set; } = string.Empty;

        /// <summary>
        /// C�digo de identifica��o do Google (ex: sub do token ID).
        /// </summary>
        /// <example>109876543210987654321</example>
        [Required(ErrorMessage = "O c�digo Google � obrigat�rio.")]
        [MaxLength(120, ErrorMessage = "O c�digo Google deve ter no m�ximo 120 caracteres.")]
        public string DsCodigoGoogle { get; set; } = string.Empty; // Ou o que quer que isso represente

        /// <summary>
        /// ID do tipo de usu�rio.
        /// </summary>
        /// <example>1</example>
        [Required(ErrorMessage = "O ID do tipo de usu�rio � obrigat�rio.")]
        public int IdTipoUsuario { get; set; }

        /// <summary>
        /// ID da pessoa associada a este usu�rio.
        /// </summary>
        /// <example>101</example>
        [Required(ErrorMessage = "O ID da pessoa � obrigat�rio.")]
        public int IdPessoa { get; set; } // ID da Pessoa associada
    }

    /// <summary>
    /// DTO para atualiza��o de um usu�rio existente.
    /// </summary>
    public class UsuarioUpdateDto // DTO para atualiza��o de usu�rio
    {
        /// <summary>
        /// Novo nome do usu�rio (opcional).
        /// </summary>
        /// <example>Jos� Almeida</example>
        [MaxLength(100, ErrorMessage = "O nome de usu�rio deve ter no m�ximo 100 caracteres.")]
        public string? NmUsuario { get; set; }

        /// <summary>
        /// Novo endere�o de e-mail do usu�rio (opcional). Deve ser �nico.
        /// </summary>
        /// <example>jose.almeida@example.com</example>
        [MaxLength(100, ErrorMessage = "O e-mail deve ter no m�ximo 100 caracteres.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inv�lido.")]
        public string? DsEmail { get; set; }

        /// <summary>
        /// Nova senha do usu�rio (opcional).
        /// </summary>
        /// <example>NovaSenha@456</example>
        [MinLength(6, ErrorMessage = "A senha deve ter no m�nimo 6 caracteres.")]
        public string? DsSenha { get; set; } // Nullable

        /// <summary>
        /// Novo c�digo de identifica��o do Google (opcional).
        /// </summary>
        /// <example>210987654321098765432</example>
        [MaxLength(120, ErrorMessage = "O c�digo Google deve ter no m�ximo 120 caracteres.")]
        public string? DsCodigoGoogle { get; set; }

        /// <summary>
        /// Novo ID do tipo de usu�rio (opcional).
        /// </summary>
        /// <example>2</example>
        public int? IdTipoUsuario { get; set; }
        // IdPessoa geralmente n�o � alterado ap�s a cria��o do usu�rio
    }

    /// <summary>
    /// DTO para visualiza��o de dados de um usu�rio.
    /// </summary>
    public class UsuarioGetDto : ResourceBaseDto // DTO para retornar informa��es do Usu�rio
    {
        /// <summary>
        /// ID �nico do usu�rio.
        /// </summary>
        /// <example>15</example>
        public int IdUsuario { get; set; }
        /// <summary>
        /// Nome do usu�rio.
        /// </summary>
        /// <example>Maria Oliveira</example>
        public string NmUsuario { get; set; } = string.Empty;
        /// <summary>
        /// Endere�o de e-mail do usu�rio.
        /// </summary>
        /// <example>maria.oliveira@example.com</example>
        public string DsEmail { get; set; } = string.Empty;
        // N�o retorne a senha ou c�digo do Google por seguran�a
        /// <summary>
        /// ID do tipo de usu�rio.
        /// </summary>
        /// <example>1</example>
        public int IdTipoUsuario { get; set; }
        /// <summary>
        /// Detalhes do tipo de usu�rio.
        /// </summary>
        public TipoUsuario? TipoUsuario { get; set; } // Related TipoUsuario
        /// <summary>
        /// ID da pessoa associada a este usu�rio.
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
        /// Endere�o de e-mail para login.
        /// </summary>
        /// <example>admin@example.com</example>
        [Required(ErrorMessage = "O e-mail � obrigat�rio.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inv�lido.")]
        public string DsEmail { get; set; } = string.Empty;

        /// <summary>
        /// Senha para login.
        /// </summary>
        /// <example>AdminSenha123</example>
        [Required(ErrorMessage = "A senha � obrigat�ria.")]
        public string DsSenha { get; set; } = string.Empty;
    }
}