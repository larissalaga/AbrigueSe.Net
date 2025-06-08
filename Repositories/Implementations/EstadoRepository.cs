using AbrigueSe.Data;
using AbrigueSe.Models;
using AbrigueSe.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbrigueSe.Dtos; // Assuming EstadoDto exists
using AutoMapper;    // Assuming IMapper is injected

namespace AbrigueSe.Repositories.Implementations
{
    public class EstadoRepository : IEstadoRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public EstadoRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Estado> Create(EstadoDto estadoDto) // Assuming EstadoDto for creation
        {
            var paisExists = await _context.Pais.FirstOrDefaultAsync(p => p.IdPais == estadoDto.IdPais);
            if (paisExists == null)
            {
                throw new System.Exception("Pa�s associado n�o encontrado.");
            }

            var estadoByNameAndPaisExists = await _context.Estado.FirstOrDefaultAsync(e => e.NmEstado == estadoDto.NmEstado && e.IdPais == estadoDto.IdPais);
            if (estadoByNameAndPaisExists != null)
            {
                throw new System.Exception("J� existe um estado com este nome neste pa�s.");
            }

            var nextId = await _context.GetNextSequenceValueAsync("seq_t_gsab_estado"); // VERIFY SEQUENCE NAME
            var newEstado = _mapper.Map<Estado>(estadoDto);
            newEstado.IdEstado = nextId;

            _context.Estado.Add(newEstado);
            await _context.SaveChangesAsync();
            return await GetById(newEstado.IdEstado);
        }

        public async Task<bool> Delete(int id)
        {
            var estado = await _context.Estado.FirstOrDefaultAsync(e => e.IdEstado == id);
            if (estado == null)
            {
                return false;
            }

            var dependentCidadeExists = await _context.Cidade.FirstOrDefaultAsync(c => c.IdEstado == id);
            if (dependentCidadeExists != null)
            {
                throw new System.InvalidOperationException("Este estado n�o pode ser exclu�do pois possui cidades associadas.");
            }
            _context.Estado.Remove(estado);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Estado>> GetAll()
        {
            return await _context.Estado.Include(e => e.Pais).OrderBy(e => e.IdEstado).ToListAsync();
        }
        
        public async Task<List<Estado>> GetByPaisId(int idPais)
        {
            return await _context.Estado
                                 .Where(e => e.IdPais == idPais)
                                 .Include(e => e.Pais)
                                 .OrderBy(e => e.NmEstado) // Or IdEstado
                                 .ToListAsync();
        }

        public async Task<Estado> GetById(int id)
        {
            var estado = await _context.Estado.Include(e => e.Pais).FirstOrDefaultAsync(e => e.IdEstado == id);
            if (estado == null)
            {
                throw new KeyNotFoundException("Estado n�o encontrado.");
            }
            return estado;
        }

        public async Task<Estado> Update(EstadoDto estadoDto, int id) // Assuming EstadoDto for update
        {
            var estado = await _context.Estado.FirstOrDefaultAsync(e => e.IdEstado == id);
            if (estado == null)
            {
                throw new KeyNotFoundException("Estado n�o encontrado para atualiza��o.");
            }

            if (estado.IdPais != estadoDto.IdPais)
            {
                var paisExists = await _context.Pais.FirstOrDefaultAsync(p => p.IdPais == estadoDto.IdPais);
                if (paisExists == null)
                {
                    throw new System.Exception("Novo pa�s associado n�o encontrado.");
                }
            }
            
            if (estado.NmEstado != estadoDto.NmEstado || estado.IdPais != estadoDto.IdPais)
            {
                var conflictingEstadoExists = await _context.Estado.FirstOrDefaultAsync(e => e.NmEstado == estadoDto.NmEstado && e.IdPais == estadoDto.IdPais && e.IdEstado != id);
                if (conflictingEstadoExists != null)
                {
                     throw new System.Exception("J� existe outro estado com este nome neste pa�s.");
                }
            }

            _mapper.Map(estadoDto, estado);
            await _context.SaveChangesAsync();
            return await GetById(id);
        }
    }
}