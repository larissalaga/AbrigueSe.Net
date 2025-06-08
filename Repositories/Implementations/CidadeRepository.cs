using AbrigueSe.Data;
using AbrigueSe.Models;
using AbrigueSe.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbrigueSe.Dtos; // Assuming CidadeDto exists
using AutoMapper;    // Assuming IMapper is injected

namespace AbrigueSe.Repositories.Implementations
{
    public class CidadeRepository : ICidadeRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public CidadeRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Cidade> Create(CidadeDto cidadeDto) // Assuming CidadeDto for creation
        {
            var estadoExists = await _context.Estado.FirstOrDefaultAsync(e => e.IdEstado == cidadeDto.IdEstado);
            if (estadoExists == null)
            {
                throw new System.Exception("Estado associado não encontrado.");
            }

            var cidadeByNameAndEstadoExists = await _context.Cidade.FirstOrDefaultAsync(c => c.NmCidade == cidadeDto.NmCidade && c.IdEstado == cidadeDto.IdEstado);
            if (cidadeByNameAndEstadoExists != null)
            {
                throw new System.Exception("Já existe uma cidade com este nome neste estado.");
            }

            var nextId = await _context.GetNextSequenceValueAsync("seq_t_gsab_cidade"); // VERIFY SEQUENCE NAME
            var newCidade = _mapper.Map<Cidade>(cidadeDto);
            newCidade.IdCidade = nextId;
            
            _context.Cidade.Add(newCidade);
            await _context.SaveChangesAsync();
            return await GetById(newCidade.IdCidade);
        }

        public async Task<bool> Delete(int id)
        {
            var cidade = await _context.Cidade.FirstOrDefaultAsync(c => c.IdCidade == id);
            if (cidade == null)
            {
                return false;
            }

            var dependentEnderecoExists = await _context.Endereco.FirstOrDefaultAsync(e => e.IdCidade == id);
            if (dependentEnderecoExists != null)
            {
                throw new System.InvalidOperationException("Esta cidade não pode ser excluída pois possui endereços associados.");
            }
            _context.Cidade.Remove(cidade);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Cidade>> GetAll()
        {
            return await _context.Cidade
                                 .Include(c => c.Estado)
                                 .ThenInclude(e => e.Pais)
                                 .OrderBy(c => c.IdCidade)
                                 .ToListAsync();
        }

        public async Task<List<Cidade>> GetByEstadoId(int idEstado)
        {
            return await _context.Cidade
                                 .Where(c => c.IdEstado == idEstado)
                                 .Include(c => c.Estado)
                                 .OrderBy(c => c.NmCidade) // Or IdCidade
                                 .ToListAsync();
        }
        
        public async Task<Cidade> GetById(int id)
        {
            var cidade = await _context.Cidade
                                       .Include(c => c.Estado)
                                       .ThenInclude(e => e.Pais)
                                       .FirstOrDefaultAsync(c => c.IdCidade == id);
            if (cidade == null)
            {
                throw new KeyNotFoundException("Cidade não encontrada.");
            }
            return cidade;
        }

        public async Task<Cidade> Update(CidadeDto cidadeDto, int id) // Assuming CidadeDto for update
        {
            var cidade = await _context.Cidade.FirstOrDefaultAsync(c => c.IdCidade == id);
            if (cidade == null)
            {
                throw new KeyNotFoundException("Cidade não encontrada para atualização.");
            }

            if (cidade.IdEstado != cidadeDto.IdEstado)
            {
                var estadoExists = await _context.Estado.FirstOrDefaultAsync(e => e.IdEstado == cidadeDto.IdEstado);
                if (estadoExists == null)
                {
                    throw new System.Exception("Novo estado associado não encontrado.");
                }
            }
            
            if (cidade.NmCidade != cidadeDto.NmCidade || cidade.IdEstado != cidadeDto.IdEstado)
            {
                var conflictingCidadeExists = await _context.Cidade.FirstOrDefaultAsync(c => c.NmCidade == cidadeDto.NmCidade && c.IdEstado == cidadeDto.IdEstado && c.IdCidade != id);
                if (conflictingCidadeExists != null)
                {
                    throw new System.Exception("Já existe outra cidade com este nome neste estado.");
                }
            }

            _mapper.Map(cidadeDto, cidade);
            await _context.SaveChangesAsync();
            return await GetById(id);
        }
    }
}