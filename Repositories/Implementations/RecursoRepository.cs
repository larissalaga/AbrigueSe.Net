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
    public class RecursoRepository : IRecursoRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public RecursoRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        private async Task PopulateRecursoDetails(Recurso recurso)
        {
            if (recurso == null) return;

            /*recurso = await _context.EstoqueRecurso
                .Where(es => es.IdRecurso == recurso.IdRecurso)
                .OrderByDescending(es => es.DtAtualizacao)
                .Include(es => es.Abrigo) // Include related Abrigo for UltimoEstoque
                .FirstOrDefaultAsync();*/
        }

        public async Task<Recurso> Create(RecursoDto recursoDto)
        {
            var recursoByDescExists = await _context.Recurso.FirstOrDefaultAsync(r => r.DsRecurso == recursoDto.DsRecurso);
            if (recursoByDescExists != null)
            {
                throw new Exception("Já existe um recurso com esta descrição.");
            }

            var nextId = await _context.GetNextSequenceValueAsync("seq_t_gsab_recurso"); 
            var newRecurso = _mapper.Map<Recurso>(recursoDto);
            newRecurso.IdRecurso = nextId;

            _context.Recurso.Add(newRecurso);
            await _context.SaveChangesAsync();
            return await GetById(newRecurso.IdRecurso); 
        }

        public async Task<bool> DeleteById(int id)
        {
            var recurso = await _context.Recurso.FirstOrDefaultAsync(r => r.IdRecurso == id);
            if (recurso == null)
            {
                return false;
            }

            var isInEstoque = await _context.EstoqueRecurso.FirstOrDefaultAsync(er => er.IdRecurso == id);
            if (isInEstoque != null)
            {
                throw new Exception("Este recurso não pode ser excluído pois está associado a um estoque.");
            }

            _context.Recurso.Remove(recurso);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Recurso>> GetAll()
        {
            var recursos = await _context.Recurso.OrderBy(r => r.IdRecurso).ToListAsync();
            if (recursos == null || !recursos.Any()) 
                throw new Exception("Nenhum recurso encontrado.");

            foreach(var recurso in recursos)
            {
                await PopulateRecursoDetails(recurso);
            }
            return recursos;
        }

        public async Task<Recurso> GetById(int id)
        {
            var recurso = await _context.Recurso.FirstOrDefaultAsync(r => r.IdRecurso == id);
            if (recurso == null)
            {
                throw new KeyNotFoundException($"Recurso com ID {id} não encontrado."); 
            }
            await PopulateRecursoDetails(recurso);
            return recurso;
        }

        public async Task<Recurso> UpdateById(int id, RecursoDto recursoDto)
        {
            var recurso = await _context.Recurso.FirstOrDefaultAsync(r => r.IdRecurso == id);
            if (recurso == null)
            {
                throw new KeyNotFoundException($"Recurso com ID {id} não encontrado para atualização."); 
            }

            if (recurso.DsRecurso != recursoDto.DsRecurso)
            {
                var conflictingRecursoDescExists = await _context.Recurso.FirstOrDefaultAsync(r => r.DsRecurso == recursoDto.DsRecurso && r.IdRecurso != id);
                if (conflictingRecursoDescExists != null)
                {
                    throw new Exception("Já existe outro recurso com esta descrição.");
                }
            }

            _mapper.Map(recursoDto, recurso);
            await _context.SaveChangesAsync();
            return await GetById(id); 
        }
    }
}