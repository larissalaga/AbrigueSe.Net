using System.ComponentModel.DataAnnotations;

namespace AbrigueSe.Dtos
{
    public class UsuarioCreateDto // DTO para criação de usuário
    {
        [Required(ErrorMessage = "O nome de usuário é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O nome de usuário deve ter no máximo 100 caracteres.")]
        public string NmUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O e-mail deve ter no máximo 100 caracteres.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        public string DsEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")] // Exemplo de validação de senha
        [MaxLength(100, ErrorMessage = "A senha deve ter no máximo 100 caracteres.")]
        public string DsSenha { get; set; } = string.Empty;

        [Required(ErrorMessage = "O código Google é obrigatório.")]
        [MaxLength(120, ErrorMessage = "O código Google deve ter no máximo 120 caracteres.")]
        public string DsCodigoGoogle { get; set; } = string.Empty; // Ou o que quer que isso represente

        [Required(ErrorMessage = "O ID do tipo de usuário é obrigatório.")]
        public int IdTipoUsuario { get; set; }

        [Required(ErrorMessage = "O ID da pessoa é obrigatório.")]
        public int IdPessoa { get; set; } // ID da Pessoa associada
    }

    public class UsuarioUpdateDto // DTO para atualização de usuário
    {
        [MaxLength(100, ErrorMessage = "O nome de usuário deve ter no máximo 100 caracteres.")]
        public string? NmUsuario { get; set; }

        [MaxLength(100, ErrorMessage = "O e-mail deve ter no máximo 100 caracteres.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        public string? DsEmail { get; set; }

        [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
        public string? DsSenha { get; set; } // Nullable

        [MaxLength(120, ErrorMessage = "O código Google deve ter no máximo 120 caracteres.")]
        public string? DsCodigoGoogle { get; set; }

        public int? IdTipoUsuario { get; set; }
        // IdPessoa geralmente não é alterado após a criação do usuário
    }

    public class UsuarioGetDto // DTO para retornar informações do Usuário
    {
        public int IdUsuario { get; set; }
        public string NmUsuario { get; set; } = string.Empty;
        public string DsEmail { get; set; } = string.Empty;
        // Não retorne a senha ou código do Google por segurança
        public int IdTipoUsuario { get; set; }
        public TipoUsuarioGetDto? TipoUsuario { get; set; } // Related TipoUsuario
        public int IdPessoa { get; set; }
        public PessoaGetDto? Pessoa { get; set; } // Detalhes da pessoa associada
    }

    public class LoginDto // DTO para login
    {
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        public string DsEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória.")]
        public string DsSenha { get; set; } = string.Empty;
    }
}