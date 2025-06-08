using AbrigueSe.Dtos;
using AbrigueSe.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AbrigueSe.Repositories.Interfaces
{
    public interface IAbrigoRepository
    {
        Task<List<Abrigo>> GetAll();
        Task<Abrigo> GetById(int id);        
        Task<Abrigo> Create(AbrigoCreateDto abrigoDto);
        Task<Abrigo> UpdateById(int id, AbrigoCreateDto abrigoDto);
        Task<bool> DeleteById(int id);
        Task<AbrigoGetDto> GetDetailsByIdAsync(int id); // New method for detailed view
    }
}
