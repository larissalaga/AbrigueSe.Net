using AbrigueSe.Dtos;
using AbrigueSe.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AbrigueSe.Repositories.Interfaces
{
    public interface IPaisRepository
    {
        Task<List<Pais>> GetAll();
        Task<Pais> GetById(int id);
        Task<Pais> Create(PaisDto paisDto); // Changed to Task<Pais>
        Task<Pais> Update(PaisDto paisDto, int id);  // Changed to Task<Pais>
        Task<bool> Delete(int id);
    }
}