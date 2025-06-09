using Microsoft.EntityFrameworkCore;
using AbrigueSe.Data;
using AbrigueSe.Dtos;
using AbrigueSe.Models;
using AbrigueSe.Repositories.Interfaces;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AbrigueSe.Repositories.Implementations
{
    public class AbrigoRepository : IAbrigoRepository
    {
        private readonly DataContext _context;                
        private readonly IMapper _mapper;

        public AbrigoRepository(DataContext context, IMapper mapper)
        {
            _context = context;            
            _mapper = mapper;
        }

        public async Task<Abrigo> Create(AbrigoCreateDto abrigoDto)
        {
            // Validate if Endereco exists
            var enderecoExists = await _context.Endereco.FirstOrDefaultAsync(e => e.IdEndereco == abrigoDto.IdEndereco);
            if (enderecoExists == null)
            {
                throw new Exception("Endereço especificado não encontrado.");
            }

            // Check if Abrigo with the same name already exists
            var abrigoByNameExists = await _context.Abrigo.FirstOrDefaultAsync(x => x.NmAbrigo == abrigoDto.NmAbrigo);
            if (abrigoByNameExists != null)
            {
                throw new Exception("Já existe um abrigo com esse nome.");
            }

            // Check if Abrigo with the same name already exists
            var abrigoByEndereco = await _context.Abrigo.FirstOrDefaultAsync(x => x.IdEndereco == abrigoDto.IdEndereco);
            if (abrigoByEndereco != null)
            {
                throw new Exception("Já existe um abrigo com esse endereço.");
            }

            var nextId = await _context.GetNextSequenceValueAsync("seq_t_gsab_abrigo"); 
            var newAbrigo = _mapper.Map<Abrigo>(abrigoDto);
            newAbrigo.IdAbrigo = nextId;
            newAbrigo.NrOcupacaoAtual = 0; 
            
            _context.Abrigo.Add(newAbrigo);
            await _context.SaveChangesAsync();
            return await GetById(newAbrigo.IdAbrigo);
        }

        public async Task<bool> DeleteById(int id)
        {
            var getAbrigo = await _context.Abrigo.FirstOrDefaultAsync(x => x.IdAbrigo == id);
            if (getAbrigo == null)
            {
                return false;
            }
            
            var activeCheckInExists = await _context.CheckIn.FirstOrDefaultAsync(ci => ci.IdAbrigo == id && ci.DtSaida == null);
            if (activeCheckInExists != null)
            {
                throw new Exception("Não é possível excluir o abrigo pois existem check-ins ativos associados.");
            }

            _context.Abrigo.Remove(getAbrigo);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Abrigo>> GetAll()
        {
            var abrigos = await _context.Abrigo
                                      .Include(a => a.Endereco) 
                                      .OrderBy(a => a.IdAbrigo)
                                      .ToListAsync();
            if (abrigos == null || !abrigos.Any()) 
                throw new Exception("Nenhum abrigo encontrado.");
            return abrigos;
        }

        public async Task<Abrigo> GetById(int id)
        {
            var abrigo = await _context.Abrigo
                                     .Include(a => a.Endereco) 
                                        .ThenInclude(e => e.Cidade)
                                            .ThenInclude(c => c.Estado)
                                                .ThenInclude(es => es.Pais)
                                     .FirstOrDefaultAsync(x => x.IdAbrigo == id);
            if (abrigo == null)
            {
                throw new Exception("Abrigo não encontrado.");
            }
            return abrigo;
        }

        public async Task<AbrigoGetDto> GetDetailsByIdAsync(int id)
        {
            var abrigoModel = await _context.Abrigo
                                     .Include(a => a.Endereco)
                                        .ThenInclude(e => e.Cidade)
                                            .ThenInclude(c => c.Estado)
                                                .ThenInclude(es => es.Pais)                                     
                                     .AsNoTracking()
                                     .FirstOrDefaultAsync(x => x.IdAbrigo == id);

            if (abrigoModel == null)
            {
                throw new KeyNotFoundException($"Abrigo com ID {id} não encontrado.");
            }

            var abrigoGetDto = _mapper.Map<AbrigoGetDto>(abrigoModel);

            // Buscar os últimos check-ins ativos para cada pessoa neste abrigo
            var ultimosCheckInsAtivos = await _context.CheckIn
                .Where(ci => ci.IdAbrigo == id && ci.DtSaida == null)
                .Include(ci => ci.Pessoa) // Incluir a Pessoa associada ao CheckIn
                .GroupBy(ci => ci.IdPessoa)
                .Select(g => g.OrderByDescending(ci => ci.DtEntrada).FirstOrDefault())
                .ToListAsync();

            // Se houver check-ins, popule as listas no DTO
            if (ultimosCheckInsAtivos != null && ultimosCheckInsAtivos.Any())
            {
                //abrigoGetDto.CheckIns = ultimosCheckInsAtivos.Where(ci => ci != null).ToList();
                abrigoGetDto.Pessoas = ultimosCheckInsAtivos.Where(ci => ci != null && ci.Pessoa != null).Select(ci => ci.Pessoa).Distinct().ToList();
            }
            else
            {
                //abrigoGetDto.CheckIns = new List<CheckIn>();
                abrigoGetDto.Pessoas = new List<Pessoa>();
            }

            // Step 1: Get the IDs of the latest EstoqueRecurso for each Recurso in the Abrigo
            var latestEstoqueRecursoIds = await _context.EstoqueRecurso
                .Where(er => er.IdAbrigo == id)
                .GroupBy(er => er.IdRecurso)
                .Select(g => g.OrderByDescending(er => er.DtAtualizacao)
                              .ThenByDescending(er => er.IdEstoque) // Ensure deterministic latest
                              .Select(er => er.IdEstoque) // Select only the ID
                              .First())
                .ToListAsync();

            // Step 2: Fetch these EstoqueRecurso entities and include their Recurso
            if (latestEstoqueRecursoIds.Any())
            {
                var estoqueRecursos = await _context.EstoqueRecurso
                    .Where(er => latestEstoqueRecursoIds.Contains(er.IdEstoque))
                    .Include(er => er.Recurso) // Include the Recurso navigation property
                    .OrderByDescending(er => er.DtAtualizacao)
                    .ToListAsync();
                abrigoGetDto.EstoqueRecursos = estoqueRecursos;
            }
            else
            {
                abrigoGetDto.EstoqueRecursos = new List<EstoqueRecurso>();
            }


            if (abrigoGetDto.EstoqueRecursos == null) // Should be initialized by the logic above
            {
                abrigoGetDto.EstoqueRecursos = new List<EstoqueRecurso>();
            }

            return abrigoGetDto;
        }
        public async Task<List<Pessoa>> GetPessoasAtivasByAbrigoIdAsync(int id)
        {
            var abrigoModel = await _context.Abrigo
                                     .Include(a => a.Endereco)
                                        .ThenInclude(e => e.Cidade)
                                            .ThenInclude(c => c.Estado)
                                                .ThenInclude(es => es.Pais)
                                     .AsNoTracking()
                                     .FirstOrDefaultAsync(x => x.IdAbrigo == id);

            if (abrigoModel == null)
            {
                throw new KeyNotFoundException($"Abrigo com ID {id} não encontrado.");
            }

            var abrigoGetDto = _mapper.Map<AbrigoGetDto>(abrigoModel);

            // Buscar os últimos check-ins ativos para cada pessoa neste abrigo
            var ultimosCheckInsAtivos = await _context.CheckIn
                .Where(ci => ci.IdAbrigo == id && ci.DtSaida == null)
                .Include(ci => ci.Pessoa) // Incluir a Pessoa associada ao CheckIn
                .GroupBy(ci => ci.IdPessoa)
                .Select(g => g.OrderByDescending(ci => ci.DtEntrada).FirstOrDefault())
                .ToListAsync();

            // Se houver check-ins, popule as listas no DTO
            if (ultimosCheckInsAtivos != null && ultimosCheckInsAtivos.Any())
            {
                //abrigoGetDto.CheckIns = ultimosCheckInsAtivos.Where(ci => ci != null).ToList();
                abrigoGetDto.Pessoas = ultimosCheckInsAtivos.Where(ci => ci != null && ci.Pessoa != null).Select(ci => ci.Pessoa).Distinct().ToList();
            }
            else
            {
                //abrigoGetDto.CheckIns = new List<CheckIn>();
                abrigoGetDto.Pessoas = new List<Pessoa>();
            }
            return abrigoGetDto.Pessoas;
        }

        public async Task<Abrigo> UpdateById(int id, AbrigoCreateDto abrigoDto)
        {
            var getAbrigo = await _context.Abrigo.FindAsync(id); 
            if (getAbrigo == null)
            {
                throw new Exception("Abrigo não encontrado.");
            }

            if (getAbrigo.IdEndereco != abrigoDto.IdEndereco)
            {
                var newEnderecoExists = await _context.Endereco.FirstOrDefaultAsync(e => e.IdEndereco == abrigoDto.IdEndereco);
                if (newEnderecoExists == null)
                {
                    throw new Exception("Novo endereço especificado não encontrado.");
                }
            }
            
            if (getAbrigo.NmAbrigo != abrigoDto.NmAbrigo)
            {
                var conflictingAbrigoNameExists = await _context.Abrigo.FirstOrDefaultAsync(x => x.NmAbrigo == abrigoDto.NmAbrigo && x.IdAbrigo != id);
                if (conflictingAbrigoNameExists != null)
                {
                    throw new Exception("Já existe outro abrigo com este novo nome.");
                }
            }

            _mapper.Map(abrigoDto, getAbrigo);
            
            await _context.SaveChangesAsync();
            return await GetById(id); 
        }
    }
}
