using AbrigueSe.Models;
using AbrigueSe.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AbrigueSe.Repositories.Interfaces
{
    public interface IRecursoRepository
    {
        Task<List<Recurso>> GetAll();
        Task<Recurso> GetById(int id);
        Task<Recurso> Create(RecursoDto recursoDto);
        Task<Recurso> UpdateById(int id, RecursoDto recursoDto);
        Task<bool> DeleteById(int id);
    }
}