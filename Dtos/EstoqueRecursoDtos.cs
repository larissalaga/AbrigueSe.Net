using AbrigueSe.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace AbrigueSe.Dtos
{
    public class EstoqueRecursoDto // For Create/Update
    {
        [Required(ErrorMessage = "A quantidade disponível é obrigatória.")]
        [Range(0, int.MaxValue, ErrorMessage = "A quantidade disponível não pode ser negativa.")]
        public int QtDisponivel { get; set; }

        // DtAtualizacao is usually set by the server on create/update
        // public DateTime DtAtualizacao { get; set; } 

        [Required(ErrorMessage = "O ID do abrigo é obrigatório.")]
        public int IdAbrigo { get; set; }

        [Required(ErrorMessage = "O ID do recurso é obrigatório.")]
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