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
    public class EnderecoRepository : IEnderecoRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public EnderecoRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Endereco> Create(EnderecoDto enderecoDto)
        {
            var cidadeExists = await _context.Cidade.FirstOrDefaultAsync(c => c.IdCidade == enderecoDto.IdCidade);
            if (cidadeExists == null)
            {
                throw new Exception("Cidade associada não encontrada.");
            }

            var nextId = await _context.GetNextSequenceValueAsync("seq_t_gsab_endereco");
            var newEndereco = _mapper.Map<Endereco>(enderecoDto);
            newEndereco.IdEndereco = nextId;

            _context.Endereco.Add(newEndereco);
            await _context.SaveChangesAsync();
            return await GetById(newEndereco.IdEndereco); // Return fetched entity
        }

        public async Task<bool> Delete(int id)
        {
            var endereco = await _context.Endereco.FirstOrDefaultAsync(e => e.IdEndereco == id);
            if (endereco == null)
            {
                return false; 
            }
            
            var pessoaExists = await _context.Pessoa.FirstOrDefaultAsync(p => p.IdEndereco == id);
            var abrigoExists = await _context.Abrigo.FirstOrDefaultAsync(a => a.IdEndereco == id);

            if (pessoaExists != null || abrigoExists != null)
            {
                 throw new InvalidOperationException("Este endereço não pode ser excluído pois está associado a pessoas ou abrigos.");
            }
            _context.Endereco.Remove(endereco);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Endereco>> GetAll()
        {
            return await _context.Endereco
                                 .Include(e => e.Cidade)
                                    .ThenInclude(c => c.Estado)
                                        .ThenInclude(es => es.Pais)
                                 .OrderBy(e => e.IdEndereco)
                                 .ToListAsync();
        }

        public async Task<Endereco> GetById(int id)
        {
            var endereco = await _context.Endereco
                                         .Include(e => e.Cidade)
                                            .ThenInclude(c => c.Estado)
                                                .ThenInclude(es => es.Pais)
                                         .FirstOrDefaultAsync(e => e.IdEndereco == id);
            if (endereco == null)
            {
                throw new KeyNotFoundException("Endereço não encontrado.");
            }
            return endereco;
        }

        public async Task<Endereco> Update(EnderecoDto enderecoDto, int id)
        {
            var endereco = await _context.Endereco.FirstOrDefaultAsync(e => e.IdEndereco == id);
            if (endereco == null)
            {
                throw new KeyNotFoundException("Endereço não encontrado para atualização.");
            }
            if (endereco.IdCidade != enderecoDto.IdCidade)
            {
                var cidadeExists = await _context.Cidade.FirstOrDefaultAsync(c => c.IdCidade == enderecoDto.IdCidade);
                 if (cidadeExists == null)
                {
                    throw new Exception("Nova cidade associada não encontrada.");
                }
            }
            
            _mapper.Map(enderecoDto, endereco);
            await _context.SaveChangesAsync();
            return await GetById(id); // Return fetched entity
        }
    }
}