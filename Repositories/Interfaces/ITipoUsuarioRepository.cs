using AbrigueSe.Models;
using AbrigueSe.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AbrigueSe.Repositories.Interfaces
{
    public interface ITipoUsuarioRepository
    {
        Task<List<TipoUsuario>> GetAll();
        Task<TipoUsuario> GetById(int id);
        Task<TipoUsuario> Create(TipoUsuarioDto tipoUsuarioDto);
        Task<TipoUsuario> UpdateById(int id, TipoUsuarioDto tipoUsuarioDto);
        Task<bool> DeleteById(int id);
    }
}