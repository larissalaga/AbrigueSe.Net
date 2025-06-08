using AbrigueSe.Data;
using AbrigueSe.Dtos;
using AbrigueSe.Models;
using AbrigueSe.Repositories.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AbrigueSe.Repositories.Implementations
{
    public class CheckInRepository : ICheckInRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public CheckInRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CheckIn> Create(CheckInDto checkInDto)
        {
            var abrigo = await _context.Abrigo.FirstOrDefaultAsync(a => a.IdAbrigo == checkInDto.IdAbrigo);
            if (abrigo == null) throw new Exception("Abrigo não encontrado.");

            var pessoa = await _context.Pessoa.FirstOrDefaultAsync(p => p.IdPessoa == checkInDto.IdPessoa);
            if (pessoa == null) throw new Exception("Pessoa não encontrada.");

            var activeCheckin = await GetActiveCheckInByPessoaId(checkInDto.IdPessoa);
            if (activeCheckin != null)
            {
                var abrigoDoCheckinAtivo = await _context.Abrigo.FirstOrDefaultAsync(a => a.IdAbrigo == activeCheckin.IdAbrigo);
                throw new Exception($"Esta pessoa já possui um check-in ativo no abrigo '{abrigoDoCheckinAtivo?.NmAbrigo ?? activeCheckin.IdAbrigo.ToString()}' desde {activeCheckin.DtEntrada}.");
            }
            
            if (abrigo.NrOcupacaoAtual >= abrigo.NrCapacidade)
            {
                throw new Exception($"O abrigo '{abrigo.NmAbrigo}' atingiu sua capacidade máxima de {abrigo.NrCapacidade} pessoas.");
            }

            var nextId = await _context.GetNextSequenceValueAsync("seq_t_gsab_check_in");
            var newCheckIn = _mapper.Map<CheckIn>(checkInDto);
            newCheckIn.IdCheckin = nextId;
            if (newCheckIn.DtEntrada == default) newCheckIn.DtEntrada = DateTime.UtcNow;

            _context.CheckIn.Add(newCheckIn);
            
            abrigo.NrOcupacaoAtual++;
            _context.Abrigo.Update(abrigo);

            await _context.SaveChangesAsync();
            return await GetById(newCheckIn.IdCheckin); // Return fetched entity
        }

        public async Task<bool> DeleteById(int idCheckin)
        {
            var checkIn = await _context.CheckIn.FirstOrDefaultAsync(ci => ci.IdCheckin == idCheckin);
            if (checkIn == null)
            {
                return false; 
            }

            if (checkIn.DtSaida == null)
            {
                var abrigo = await _context.Abrigo.FirstOrDefaultAsync(a => a.IdAbrigo == checkIn.IdAbrigo);
                if (abrigo != null)
                {
                    abrigo.NrOcupacaoAtual = Math.Max(0, abrigo.NrOcupacaoAtual - 1);
                    _context.Abrigo.Update(abrigo);
                }
            }

            _context.CheckIn.Remove(checkIn);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<CheckIn>> GetAll()
        {
            return await _context.CheckIn
                                 .Include(ci => ci.Abrigo)
                                 .Include(ci => ci.Pessoa)
                                 .OrderBy(ci => ci.IdCheckin)
                                 .ToListAsync();
        }

        public async Task<CheckIn> GetById(int idCheckin)
        {
            var checkIn = await _context.CheckIn
                                        .Include(ci => ci.Abrigo)
                                        .Include(ci => ci.Pessoa)
                                        .FirstOrDefaultAsync(ci => ci.IdCheckin == idCheckin);

            if (checkIn == null)
            {
                throw new KeyNotFoundException($"Check-in com ID {idCheckin} não encontrado.");
            }
            return checkIn;
        }

        public async Task<CheckIn> GetActiveCheckInByPessoaId(int idPessoa)
        {
            return await _context.CheckIn
                .Include(ci => ci.Abrigo)
                .Where(ci => ci.IdPessoa == idPessoa && ci.DtSaida == null)
                .OrderByDescending(ci => ci.DtEntrada)
                .FirstOrDefaultAsync();
        }

        public async Task<CheckIn> UpdateById(int idCheckin, CheckInDto checkInDto)
        {
            var checkIn = await _context.CheckIn.FirstOrDefaultAsync(ci => ci.IdCheckin == idCheckin);
            if (checkIn == null) throw new KeyNotFoundException($"Check-in com ID {idCheckin} não encontrado para atualização.");

            Abrigo? abrigo = null;
            if (checkInDto.IdAbrigo != checkIn.IdAbrigo) // If DTO changes the AbrigoId
            {
                var newAbrigo = await _context.Abrigo.FirstOrDefaultAsync(a => a.IdAbrigo == checkInDto.IdAbrigo);
                if (newAbrigo == null) throw new Exception($"Novo abrigo com ID {checkInDto.IdAbrigo} não encontrado.");

                if (checkIn.DtSaida == null) // If checkIn was active in old abrigo
                {
                    var oldAbrigo = await _context.Abrigo.FirstOrDefaultAsync(a => a.IdAbrigo == checkIn.IdAbrigo);
                    if (oldAbrigo != null) oldAbrigo.NrOcupacaoAtual = Math.Max(0, oldAbrigo.NrOcupacaoAtual - 1);
                }
                abrigo = newAbrigo;
            }
            else
            {
                abrigo = await _context.Abrigo.FirstOrDefaultAsync(a => a.IdAbrigo == checkIn.IdAbrigo);
            }

            if (abrigo == null) throw new Exception("Abrigo associado ao check-in não processado corretamente.");

            bool wasActive = checkIn.DtSaida == null;
            DateTime? originalDtSaida = checkIn.DtSaida;

            _mapper.Map(checkInDto, checkIn);

            bool isNowActive = checkIn.DtSaida == null;

            if (wasActive && !isNowActive)
            {
                abrigo.NrOcupacaoAtual = Math.Max(0, abrigo.NrOcupacaoAtual - 1);
            }
            else if (!wasActive && isNowActive)
            {
                if (abrigo.NrOcupacaoAtual < abrigo.NrCapacidade)
                {
                    abrigo.NrOcupacaoAtual++;
                }
                else
                {
                    checkIn.DtSaida = originalDtSaida;
                    throw new Exception($"O abrigo '{abrigo.NmAbrigo}' atingiu sua capacidade máxima. Não é possível reativar o check-in.");
                }
            }
            _context.Abrigo.Update(abrigo);

            await _context.SaveChangesAsync();
            return checkIn;
        }
    }
}