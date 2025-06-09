using AbrigueSe.Models;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; // Para List

namespace AbrigueSe.Dtos
{
    /// <summary>
    /// DTO para criação e atualização de um tipo de recurso (suprimento).
    /// </summary>
    public class RecursoDto : ResourceBaseDto // DTO para criação e atualização de Recurso
    {
        /// <summary>
        /// Descrição do recurso.
        /// </summary>
        /// <example>Água Mineral 500ml</example>
        [Required(ErrorMessage = "A descrição do recurso é obrigatória.")]
        [MaxLength(255, ErrorMessage = "A descrição deve ter no máximo 255 caracteres.")]
        public string DsRecurso { get; set; } = string.Empty;

        /// <summary>
        /// Unidade de medida do recurso.
        /// </summary>
        /// <example>Unidade</example>
        [Required(ErrorMessage = "A unidade de medida é obrigatória.")]
        [MaxLength(50, ErrorMessage = "A unidade de medida deve ter no máximo 50 caracteres.")]
        public string DsUnidadeMedida { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para visualização de dados de um tipo de recurso.
    /// </summary>
    public class RecursoGetDto : ResourceBaseDto // DTO para retornar informações do Recurso
    {
        /// <summary>
        /// ID único do tipo de recurso.
        /// </summary>
        /// <example>1</example>
        public int IdRecurso { get; set; }
        /// <summary>
        /// Descrição do recurso.
        /// </summary>
        /// <example>Colchão Solteiro</example>
        public string DsRecurso { get; set; } = string.Empty;
        /// <summary>
        /// Unidade de medida do recurso.
        /// </summary>
        /// <example>Peça</example>
        public string DsUnidadeMedida { get; set; } = string.Empty;
        // Se houver informações de estoque relacionadas que devam ser incluídas, adicione aqui.
        
        public List<EstoqueRecurso> Estoques { get; set; }
    }
}

