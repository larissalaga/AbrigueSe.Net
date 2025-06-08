using AbrigueSe.Dtos;
using AbrigueSe.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AbrigueSe.Repositories.Interfaces
{
    public interface IEstadoRepository
    {
        Task<List<Estado>> GetAll();
        Task<Estado> GetById(int id);
        Task<List<Estado>> GetByPaisId(int idPais);
        Task<Estado> Create(EstadoDto estadoDto); // Changed to Task<Estado>
        Task<Estado> Update(EstadoDto estadoDto, int id);  // Changed to Task<Estado>
        Task<bool> Delete(int id);
    }
}