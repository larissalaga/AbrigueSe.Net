using AbrigueSe.Dtos;
using AbrigueSe.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AbrigueSe.Repositories.Interfaces
{
    public interface IEnderecoRepository
    {
        Task<List<Endereco>> GetAll();
        Task<Endereco> GetById(int id);
        Task<Endereco> Create(EnderecoDto enderecoDto); // Changed to return Task<Endereco>
        Task<Endereco> Update(EnderecoDto enderecoDto, int id);  // Changed to return Task<Endereco>
        Task<bool> Delete(int id);
    }
}