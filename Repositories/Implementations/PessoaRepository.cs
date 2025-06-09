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
    public class PessoaRepository : IPessoaRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public PessoaRepository(DataContext context, IMapper mapper) 
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Pessoa> Create(PessoaDto pessoaDto)
        {
            var enderecoExists = await _context.Endereco.FirstOrDefaultAsync(e => e.IdEndereco == pessoaDto.IdEndereco);
            if (enderecoExists == null)
            {
                throw new Exception("Endere�o n�o encontrado.");
            }

            var existingPessoaByCpf = await _context.Pessoa.FirstOrDefaultAsync(p => p.NrCpf == pessoaDto.NrCpf);
            if (existingPessoaByCpf != null)
            {
                throw new Exception("J� existe uma pessoa cadastrada com este CPF.");
            }
            
            var nextId = await _context.GetNextSequenceValueAsync("seq_t_gsab_pessoa");
            var pessoa = _mapper.Map<Pessoa>(pessoaDto);
            pessoa.IdPessoa = nextId;

            _context.Pessoa.Add(pessoa);
            await _context.SaveChangesAsync();
            return await GetById(pessoa.IdPessoa); 
        }

        public async Task<bool> DeleteById(int id)
        {
            var pessoa = await _context.Pessoa.FirstOrDefaultAsync(x => x.IdPessoa == id);
            if (pessoa == null)
            {
                return false;
            }

            var hasCheckIns = await _context.CheckIn.FirstOrDefaultAsync(ci => ci.IdPessoa == id);
            if (hasCheckIns != null)
            {
                throw new Exception("Esta pessoa possui registros de check-in e n�o pode ser exclu�da diretamente. Considere anonimizar ou desativar.");
            }
            
            var isUser = await _context.Usuario.FirstOrDefaultAsync(u => u.IdPessoa == id);
            if(isUser != null)
            {
                 throw new Exception("Esta pessoa � um usu�rio do sistema e n�o pode ser exclu�da diretamente.");
            }

            _context.Pessoa.Remove(pessoa);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Pessoa>> GetAll()
        {
            var pessoas = await _context.Pessoa
                                        .Include(p => p.Endereco)
                                            .ThenInclude(e => e.Cidade) 
                                                .ThenInclude(c => c.Estado)
                                                    .ThenInclude(es => es.Pais)
                                        //.Include(p => p.Usuario) // Usuario cannot be included if not a direct navigation property on Pessoa model
                                        //    .ThenInclude(u => u.TipoUsuario) 
                                        .OrderBy(p => p.IdPessoa)
                                        .ToListAsync();

            if (pessoas == null || !pessoas.Any()) 
                throw new Exception("Nenhuma pessoa encontrada.");
            
            return pessoas;
        }

        public async Task<Pessoa> GetById(int id)
        {
            var pessoa = await _context.Pessoa
                                       .Include(p => p.Endereco) 
                                            .ThenInclude(e => e.Cidade)
                                                .ThenInclude(c => c.Estado)
                                                    .ThenInclude(es => es.Pais)
                                       //.Include(p => p.Usuario) // Usuario cannot be included if not a direct navigation property on Pessoa model
                                            //.ThenInclude(u => u.TipoUsuario) 
                                       .FirstOrDefaultAsync(x => x.IdPessoa == id);
            if (pessoa == null)
            {
                throw new KeyNotFoundException($"Pessoa com ID {id} n�o encontrada.");
            }
            return pessoa;
        }

        public async Task<PessoaGetDto> GetDetailsByIdAsync(int id)
        {
            var pessoa = await _context.Pessoa
                                .Include(p => p.Endereco)
                                    .ThenInclude(e => e.Cidade)
                                        .ThenInclude(c => c.Estado)
                                            .ThenInclude(es => es.Pais)
                                .AsNoTracking()
                                .FirstOrDefaultAsync(p => p.IdPessoa == id);

            if (pessoa == null)
            {
                throw new KeyNotFoundException($"Pessoa com ID {id} n�o encontrada.");
            }

            // Mapeamento base da Pessoa para PessoaGetDto
            var pessoaGetDto = _mapper.Map<PessoaGetDto>(pessoa);

            // Buscar Usuario associado
            var usuario = await _context.Usuario
                                .Include(u => u.TipoUsuario)
                                .AsNoTracking()
                                .FirstOrDefaultAsync(u => u.IdPessoa == id);
            if (usuario != null)
            {
                pessoaGetDto.Usuario = _mapper.Map<Usuario>(usuario);
            }

            // Buscar �ltimo CheckIn e AbrigoAtual
            var ultimoCheckIn = await _context.CheckIn
                                        .Include(ci => ci.Abrigo)
                                            .ThenInclude(a => a.Endereco)
                                                .ThenInclude(e => e.Cidade)
                                                    .ThenInclude(c => c.Estado)
                                                        .ThenInclude(es => es.Pais)
                                        .Where(ci => ci.IdPessoa == id)
                                        .OrderByDescending(ci => ci.DtEntrada)
                                        .FirstOrDefaultAsync();
            
            if (ultimoCheckIn != null)
            {
                pessoaGetDto.UltimoCheckIn = _mapper.Map<CheckIn>(ultimoCheckIn);
                if (ultimoCheckIn.Abrigo != null) // Garante que o abrigo n�o � nulo
                {
                    pessoaGetDto.AbrigoAtual = _mapper.Map<Abrigo>(ultimoCheckIn.Abrigo);
                }
            }
            
            // O Endereco da Pessoa j� foi inclu�do e mapeado pelo _mapper.Map<PessoaGetDto>(pessoa)
            // se o mapeamento de Pessoa para PessoaGetDto incluir .ForMember(dest => dest.Endereco, opt => opt.MapFrom(src => src.Endereco))
            // e Endereco para EnderecoGetDto estiver configurado.

            return pessoaGetDto;
        }

        public async Task<Pessoa> UpdateById(int id, PessoaDto pessoaDto)
        {
            var pessoa = await _context.Pessoa.FirstOrDefaultAsync(x => x.IdPessoa == id);
            if (pessoa == null)
            {
                throw new KeyNotFoundException($"Pessoa com ID {id} n�o encontrada para atualiza��o.");
            }

            if (pessoa.IdEndereco != pessoaDto.IdEndereco)
            {
                var enderecoExists = await _context.Endereco.FirstOrDefaultAsync(e => e.IdEndereco == pessoaDto.IdEndereco);
                if (enderecoExists == null)
                {
                    throw new Exception("Novo endere�o n�o encontrado.");
                }
            }

            if (pessoa.NrCpf != pessoaDto.NrCpf)
            {
                var existingPessoaByCpf = await _context.Pessoa.FirstOrDefaultAsync(p => p.NrCpf == pessoaDto.NrCpf && p.IdPessoa != id);
                if (existingPessoaByCpf != null)
                {
                    throw new Exception("J� existe outra pessoa cadastrada com este CPF.");
                }
            }

            _mapper.Map(pessoaDto, pessoa);
            await _context.SaveChangesAsync();
            return await GetById(id); 
        }

        public async Task<List<Pessoa>> GetPessoasAtivasByAbrigoIdAsync(int idAbrigo)
        {
            // Primeiro, verificar se o abrigo existe para evitar processamento desnecess�rio
            // e fornecer um feedback mais claro se o abrigo em si n�o for encontrado.
            // (Esta verifica��o pode ser opcional dependendo se o chamador j� valida a exist�ncia do abrigo)
            var abrigoExists = await _context.Abrigo.AnyAsync(a => a.IdAbrigo == idAbrigo);
            if (!abrigoExists)
            {
                // Ou lan�ar uma exce��o espec�fica, ou retornar lista vazia com log.
                // Lan�ar exce��o pode ser melhor para indicar que o ID do abrigo era inv�lido.
                // throw new KeyNotFoundException($"Abrigo com ID {idAbrigo} n�o encontrado.");
                // Por ora, retornaremos lista vazia se o abrigo n�o existir,
                // consistentes com "nenhuma pessoa ativa encontrada".
                return new List<Pessoa>();
            }

            var pessoas = await _context.CheckIn
                .Where(ci => ci.IdAbrigo == idAbrigo && ci.DtSaida == null)
                // Incluir explicitamente a entidade Pessoa para garantir que seus dados sejam carregados.
                // Isso � especialmente �til se Pessoa tiver outras navega��es que voc� precise posteriormente,
                // embora para DsCondicaoMedica (um campo direto de Pessoa) o Select j� deva ser suficiente.
                .Include(ci => ci.Pessoa)
                .Select(ci => ci.Pessoa)
                // Filtrar Pessoas nulas caso haja algum problema de integridade de dados
                // ou se a rela��o n�o for obrigat�ria (o que n�o parece ser o caso aqui).
                .Where(p => p != null)
                .Distinct()
                .ToListAsync();

            // ToListAsync() nunca retorna null, retorna uma lista vazia se nada for encontrado.
            return pessoas;
        }
    }
}