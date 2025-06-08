using AbrigueSe.Data;
using AbrigueSe.Models;
using AbrigueSe.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbrigueSe.Dtos; // Assuming PaisDto exists for create
using AutoMapper;    // Assuming IMapper is injected

namespace AbrigueSe.Repositories.Implementations
{
    public class PaisRepository : IPaisRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper; // Assuming IMapper for DTO mapping

        public PaisRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Pais> Create(PaisCreateDto paisDto) // Changed to PaisCreateDto
        {
            var paisExists = await _context.Pais.FirstOrDefaultAsync(p => p.NmPais == paisDto.NmPais);
            if (paisExists != null)
            {
                throw new System.Exception("Já existe um país com este nome.");
            }

            var nextId = await _context.GetNextSequenceValueAsync("seq_t_gsab_pais"); // VERIFY SEQUENCE NAME
            var newPais = _mapper.Map<Pais>(paisDto);
            newPais.IdPais = nextId;
            
            _context.Pais.Add(newPais);
            await _context.SaveChangesAsync();

            return newPais; // Return the created Pais object
        }

        public async Task<bool> Delete(int id)
        {
            var pais = await _context.Pais.FirstOrDefaultAsync(p => p.IdPais == id);
            if (pais == null)
            {
                throw new KeyNotFoundException("País não encontrado.");
            }

            var dependentEstadoExists = await _context.Estado.FirstOrDefaultAsync(e => e.IdPais == id);
            if (dependentEstadoExists != null)
            {
                throw new System.InvalidOperationException("Este país não pode ser excluído pois possui estados associados.");
            }
            _context.Pais.Remove(pais);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Pais>> GetAll()
        {
            return await _context.Pais.OrderBy(p => p.IdPais)
                .OrderBy(p => p.NmPais)
                .ToListAsync();
        }

        public async Task<Pais> GetById(int id)
        {
            var pais = await _context.Pais.FirstOrDefaultAsync(p => p.IdPais == id);
            if (pais == null)
            {
                throw new KeyNotFoundException("País não encontrado.");
            }
            return pais;
        }

        public async Task<Pais> Update(PaisUpdateDto paisDto, int id) // Changed to PaisUpdateDto
        {
            var pais = await _context.Pais.FirstOrDefaultAsync(p => p.IdPais == id);
            if (pais == null)
            {
                throw new KeyNotFoundException("País não encontrado para atualização.");
            }

            if (pais.NmPais != paisDto.NmPais)
            {
                var conflictingPaisExists = await _context.Pais.FirstOrDefaultAsync(p => p.NmPais == paisDto.NmPais && p.IdPais != id);
                if (conflictingPaisExists != null)
                {
                    throw new System.Exception("Já existe outro país com este nome.");
                }
            }

            _mapper.Map(paisDto, pais); // Update existing pais object
            await _context.SaveChangesAsync();

            return pais; // Return the updated Pais object
        }
    }
}