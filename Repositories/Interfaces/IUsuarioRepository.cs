using AbrigueSe.Models;
using AbrigueSe.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AbrigueSe.Repositories.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<List<Usuario>> GetAll();
        Task<Usuario> GetById(int id);
        Task<Usuario> GetByLogin(string login);
        Task<Usuario> Create(UsuarioCreateDto usuarioDto);
        Task<Usuario> UpdateById(int id, UsuarioUpdateDto usuarioDto);
        Task<bool> DeleteById(int id);
    }
}