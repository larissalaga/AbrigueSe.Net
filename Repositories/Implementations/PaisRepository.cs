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

        public async Task<Pais> Create(PaisDto paisDto) // Assuming PaisDto for creation
        {
            var existingPais = await _context.Pais.FirstOrDefaultAsync(p => p.NmPais == paisDto.NmPais);
            if (existingPais != null)
            {
                throw new System.Exception("J� existe um pa�s com este nome.");
            }

            var nextId = await _context.GetNextSequenceValueAsync("seq_t_gsab_pais"); // VERIFY SEQUENCE NAME
            var newPais = _mapper.Map<Pais>(paisDto);
            newPais.IdPais = nextId;
            
            _context.Pais.Add(newPais);
            await _context.SaveChangesAsync();

            return await GetById(newPais.IdPais); // Return the created Pais object
        }

        public async Task<bool> Delete(int id)
        {
            var pais = await _context.Pais.FirstOrDefaultAsync(p => p.IdPais == id);
            if (pais == null)
            {
                return false;
            }

            var dependentEstadoExists = await _context.Estado.FirstOrDefaultAsync(e => e.IdPais == id);
            if (dependentEstadoExists != null)
            {
                throw new System.InvalidOperationException("Este pa�s n�o pode ser exclu�do pois possui estados associados.");
            }
            _context.Pais.Remove(pais);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Pais>> GetAll()
        {
            return await _context.Pais.OrderBy(p => p.IdPais).ToListAsync();
        }

        public async Task<Pais> GetById(int id)
        {
            var pais = await _context.Pais.FirstOrDefaultAsync(p => p.IdPais == id);
            if (pais == null)
            {
                throw new KeyNotFoundException("Pa�s n�o encontrado.");
            }
            return pais;
        }

        public async Task<Pais> Update(PaisDto paisDto, int id) // Assuming PaisDto for update
        {
            var pais = await _context.Pais.FirstOrDefaultAsync(p => p.IdPais == id);
            if (pais == null)
            {
                throw new KeyNotFoundException("Pa�s n�o encontrado para atualiza��o.");
            }

            if (pais.NmPais != paisDto.NmPais)
            {
                var existingPaisWithName = await _context.Pais.FirstOrDefaultAsync(p => p.NmPais == paisDto.NmPais && p.IdPais != id);
                if (existingPaisWithName != null)
                {
                    throw new System.Exception("J� existe outro pa�s com este nome.");
                }
            }

            _mapper.Map(paisDto, pais); // Update existing pais object
            await _context.SaveChangesAsync();

            return await GetById(id); // Return the updated Pais object
        }
    }
}