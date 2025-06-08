using AbrigueSe.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace AbrigueSe.Dtos
{
    public class EstoqueRecursoDto // For Create/Update
    {
        [Required(ErrorMessage = "A quantidade dispon�vel � obrigat�ria.")]
        [Range(0, int.MaxValue, ErrorMessage = "A quantidade dispon�vel n�o pode ser negativa.")]
        public int QtDisponivel { get; set; }

        // DtAtualizacao is usually set by the server on create/update
        // public DateTime DtAtualizacao { get; set; } 

        [Required(ErrorMessage = "O ID do abrigo � obrigat�rio.")]
        public int IdAbrigo { get; set; }

        [Required(ErrorMessage = "O ID do recurso � obrigat�rio.")]
        public int IdRecurso { get; set; }
    }

    public class EstoqueRecursoGetDto // For Read
    {
        public int IdEstoque { get; set; }
        public int QtDisponivel { get; set; }
        public DateTime DtAtualizacao { get; set; }
        public int IdAbrigo { get; set; }
        public Abrigo? Abrigo { get; set; } // Related Abrigo
        public int IdRecurso { get; set; }
        public Recurso? Recurso { get; set; } // Related Recurso     
    }
}