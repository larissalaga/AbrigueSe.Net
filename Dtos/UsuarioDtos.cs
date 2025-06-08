using System.ComponentModel.DataAnnotations;

namespace AbrigueSe.Dtos
{
    public class UsuarioCreateDto // DTO para cria��o de usu�rio
    {
        [Required(ErrorMessage = "O nome de usu�rio � obrigat�rio.")]
        [MaxLength(100, ErrorMessage = "O nome de usu�rio deve ter no m�ximo 100 caracteres.")]
        public string NmUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail � obrigat�rio.")]
        [MaxLength(100, ErrorMessage = "O e-mail deve ter no m�ximo 100 caracteres.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inv�lido.")]
        public string DsEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha � obrigat�ria.")]
        [MinLength(6, ErrorMessage = "A senha deve ter no m�nimo 6 caracteres.")] // Exemplo de valida��o de senha
        [MaxLength(100, ErrorMessage = "A senha deve ter no m�ximo 100 caracteres.")]
        public string DsSenha { get; set; } = string.Empty;

        [Required(ErrorMessage = "O c�digo Google � obrigat�rio.")]
        [MaxLength(120, ErrorMessage = "O c�digo Google deve ter no m�ximo 120 caracteres.")]
        public string DsCodigoGoogle { get; set; } = string.Empty; // Ou o que quer que isso represente

        [Required(ErrorMessage = "O ID do tipo de usu�rio � obrigat�rio.")]
        public int IdTipoUsuario { get; set; }

        [Required(ErrorMessage = "O ID da pessoa � obrigat�rio.")]
        public int IdPessoa { get; set; } // ID da Pessoa associada
    }

    public class UsuarioUpdateDto // DTO para atualiza��o de usu�rio
    {
        [MaxLength(100, ErrorMessage = "O nome de usu�rio deve ter no m�ximo 100 caracteres.")]
        public string? NmUsuario { get; set; }

        [MaxLength(100, ErrorMessage = "O e-mail deve ter no m�ximo 100 caracteres.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inv�lido.")]
        public string? DsEmail { get; set; }

        [MinLength(6, ErrorMessage = "A senha deve ter no m�nimo 6 caracteres.")]
        public string? DsSenha { get; set; } // Nullable

        [MaxLength(120, ErrorMessage = "O c�digo Google deve ter no m�ximo 120 caracteres.")]
        public string? DsCodigoGoogle { get; set; }

        public int? IdTipoUsuario { get; set; }
        // IdPessoa geralmente n�o � alterado ap�s a cria��o do usu�rio
    }

    public class UsuarioGetDto // DTO para retornar informa��es do Usu�rio
    {
        public int IdUsuario { get; set; }
        public string NmUsuario { get; set; } = string.Empty;
        public string DsEmail { get; set; } = string.Empty;
        // N�o retorne a senha ou c�digo do Google por seguran�a
        public int IdTipoUsuario { get; set; }
        public TipoUsuarioGetDto? TipoUsuario { get; set; } // Related TipoUsuario
        public int IdPessoa { get; set; }
        public PessoaGetDto? Pessoa { get; set; } // Detalhes da pessoa associada
    }

    public class LoginDto // DTO para login
    {
        [Required(ErrorMessage = "O e-mail � obrigat�rio.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inv�lido.")]
        public string DsEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha � obrigat�ria.")]
        public string DsSenha { get; set; } = string.Empty;
    }
}