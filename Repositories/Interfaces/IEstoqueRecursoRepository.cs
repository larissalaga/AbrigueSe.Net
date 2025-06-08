using AbrigueSe.Models;
using AbrigueSe.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AbrigueSe.Repositories.Interfaces
{
    public interface IEstoqueRecursoRepository
    {
        Task<List<EstoqueRecurso>> GetAll();
        Task<EstoqueRecurso> GetById(int idEstoque);
        Task<EstoqueRecurso> Create(EstoqueRecursoDto estoqueRecursoDto);
        Task<EstoqueRecurso> UpdateById(int idEstoque, EstoqueRecursoDto estoqueRecursoDto);
        Task<bool> DeleteById(int idEstoque);
        Task<List<EstoqueRecurso>> GetByAbrigoId(int idAbrigo);
    }
}