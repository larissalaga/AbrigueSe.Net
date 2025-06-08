using AbrigueSe.Dtos;
using AbrigueSe.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AbrigueSe.Repositories.Interfaces
{
    public interface ICidadeRepository
    {
        Task<List<Cidade>> GetAll();
        Task<Cidade> GetById(int id);
        Task<List<Cidade>> GetByEstadoId(int idEstado);
        Task<Cidade> Create(CidadeDto cidadeDto); // Changed to Task<Cidade>
        Task<Cidade> Update(CidadeDto cidadeDto, int id);  // Changed to Task<Cidade>
        Task<bool> Delete(int id);
    }
}