using AbrigueSe.Models;
using AbrigueSe.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AbrigueSe.Repositories.Interfaces
{
    public interface ICheckInRepository
    {
        Task<List<CheckIn>> GetAll();
        Task<CheckIn> GetById(int idCheckin);
        Task<CheckIn> Create(CheckInDto checkInDto);
        Task<CheckIn> UpdateById(int idCheckin, CheckInDto checkInDto);
        Task<bool> DeleteById(int idCheckin);
        Task<CheckIn> GetActiveCheckInByPessoaId(int idPessoa);
    }
}