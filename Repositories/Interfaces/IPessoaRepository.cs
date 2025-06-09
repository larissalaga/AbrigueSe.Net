using AbrigueSe.Models;
using AbrigueSe.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AbrigueSe.Repositories.Interfaces
{
    public interface IPessoaRepository
    {
        Task<List<Pessoa>> GetAll();
        Task<Pessoa> GetById(int id);
        Task<Pessoa> Create(PessoaDto pessoaDto);
        Task<Pessoa> UpdateById(int id, PessoaDto pessoaDto);
        Task<bool> DeleteById(int id);
        Task<PessoaGetDto> GetDetailsByIdAsync(int id);
        Task<List<Pessoa>> GetPessoasAtivasByAbrigoIdAsync(int idAbrigo); // Novo método
    }
}