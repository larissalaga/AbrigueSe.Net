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
    public class EstoqueRecursoRepository : IEstoqueRecursoRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public EstoqueRecursoRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<EstoqueRecurso> Create(EstoqueRecursoDto estoqueRecursoDto)
        {
            var abrigoExists = await _context.Abrigo.FirstOrDefaultAsync(a => a.IdAbrigo == estoqueRecursoDto.IdAbrigo);
            if (abrigoExists == null)
                throw new Exception("Abrigo não encontrado.");

            var recursoExists = await _context.Recurso.FirstOrDefaultAsync(r => r.IdRecurso == estoqueRecursoDto.IdRecurso);
            if (recursoExists == null)
                throw new Exception("Recurso não encontrado.");

            var existingEstoque = await _context.EstoqueRecurso.FirstOrDefaultAsync(
                er => er.IdAbrigo == estoqueRecursoDto.IdAbrigo && er.IdRecurso == estoqueRecursoDto.IdRecurso);
            if (existingEstoque != null)
            {
                throw new Exception("Já existe um estoque para este recurso neste abrigo. Considere atualizar o existente.");
            }

            var nextId = await _context.GetNextSequenceValueAsync("seq_t_gsab_estoque_recurso"); // VERIFY SEQUENCE NAME
            var newEstoque = _mapper.Map<EstoqueRecurso>(estoqueRecursoDto);
            newEstoque.IdEstoque = nextId;
            newEstoque.DtAtualizacao = DateTime.UtcNow; // Definir data de atualização

            _context.EstoqueRecurso.Add(newEstoque);
            await _context.SaveChangesAsync();
            return await GetById(newEstoque.IdEstoque); // Return fetched entity
        }

        public async Task<bool> DeleteById(int idEstoque)
        {
            var estoque = await _context.EstoqueRecurso.FirstOrDefaultAsync(er => er.IdEstoque == idEstoque);
            if (estoque == null) return false;
            _context.EstoqueRecurso.Remove(estoque);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<EstoqueRecurso>> GetAll()
        {
            return await _context.EstoqueRecurso
                                 .Include(er => er.Abrigo)
                                 .Include(er => er.Recurso)
                                 .OrderBy(er => er.IdEstoque)
                                 .ToListAsync();
        }
        
        public async Task<List<EstoqueRecurso>> GetByAbrigoId(int idAbrigo)
        {
            return await _context.EstoqueRecurso
                                 .Where(er => er.IdAbrigo == idAbrigo)
                                 .Include(er => er.Abrigo)
                                 .Include(er => er.Recurso)
                                 .OrderBy(er => er.IdEstoque) 
                                 .ToListAsync();
        }

        public async Task<EstoqueRecurso> GetById(int idEstoque)
        {
            var estoque = await _context.EstoqueRecurso
                                        .Include(er => er.Abrigo)
                                        .Include(er => er.Recurso)
                                        .FirstOrDefaultAsync(er => er.IdEstoque == idEstoque);
            if (estoque == null) throw new KeyNotFoundException("Estoque não encontrado.");
            return estoque;
        }

        public async Task<EstoqueRecurso> UpdateById(int idEstoque, EstoqueRecursoDto estoqueRecursoDto)
        {
            var estoque = await _context.EstoqueRecurso.FirstOrDefaultAsync(er => er.IdEstoque == idEstoque);
            if (estoque == null) throw new KeyNotFoundException("Estoque não encontrado.");

            if (estoque.IdAbrigo != estoqueRecursoDto.IdAbrigo || estoque.IdRecurso != estoqueRecursoDto.IdRecurso)
            {
                var abrigoExists = await _context.Abrigo.FirstOrDefaultAsync(a => a.IdAbrigo == estoqueRecursoDto.IdAbrigo);
                 if (abrigoExists == null)
                    throw new Exception("Novo abrigo não encontrado.");

                var recursoExists = await _context.Recurso.FirstOrDefaultAsync(r => r.IdRecurso == estoqueRecursoDto.IdRecurso);
                if (recursoExists == null)
                    throw new Exception("Novo recurso não encontrado.");

                var conflictingEstoque = await _context.EstoqueRecurso.FirstOrDefaultAsync(
                    er => er.IdAbrigo == estoqueRecursoDto.IdAbrigo && 
                          er.IdRecurso == estoqueRecursoDto.IdRecurso &&
                          er.IdEstoque != idEstoque); 
                if (conflictingEstoque != null) throw new Exception("Combinação de Abrigo e Recurso já existe em outro estoque.");
            }
            
            _mapper.Map(estoqueRecursoDto, estoque);
            estoque.DtAtualizacao = DateTime.UtcNow; // Atualizar data de modificação

            await _context.SaveChangesAsync();
            return await GetById(idEstoque); // Return fetched entity
        }
    }
}