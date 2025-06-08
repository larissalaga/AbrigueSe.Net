using AbrigueSe.Dtos;
using AbrigueSe.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AbrigueSe.Repositories.Interfaces
{
    public interface IPaisRepository
    {
        Task<Pais> Create(PaisCreateDto paisDto); // Changed to PaisCreateDto, returns Pais
        Task<Pais> Update(PaisUpdateDto paisDto, int id); // Changed to PaisUpdateDto, returns Pais
        Task<bool> Delete(int id);
        Task<List<Pais>> GetAll(); // Returns List<Pais>
        Task<Pais> GetById(int id); // Returns Pais
    }
}