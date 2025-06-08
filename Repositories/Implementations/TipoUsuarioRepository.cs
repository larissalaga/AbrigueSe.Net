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
    public class TipoUsuarioRepository : ITipoUsuarioRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public TipoUsuarioRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<TipoUsuario> Create(TipoUsuarioDto tipoUsuarioDto)
        {
            var tipoByDescExists = await _context.TipoUsuario.FirstOrDefaultAsync(tu => tu.DsTipoUsuario == tipoUsuarioDto.DsTipoUsuario);
            if (tipoByDescExists != null)
            {
                throw new Exception("J� existe um tipo de usu�rio com esta descri��o.");
            }

            var nextId = await _context.GetNextSequenceValueAsync("seq_t_gsab_tipo_usuario"); 
            var newTipoUsuario = _mapper.Map<TipoUsuario>(tipoUsuarioDto);
            newTipoUsuario.IdTipoUsuario = nextId;

            _context.TipoUsuario.Add(newTipoUsuario);
            await _context.SaveChangesAsync();
            return await GetById(newTipoUsuario.IdTipoUsuario); // Return fetched entity
        }

        public async Task<bool> DeleteById(int id)
        {
            var tipoUsuario = await _context.TipoUsuario.FirstOrDefaultAsync(tu => tu.IdTipoUsuario == id);
            if (tipoUsuario == null)
            {
                return false;
            }

            var isInUse = await _context.Usuario.FirstOrDefaultAsync(u => u.IdTipoUsuario == id);
            if (isInUse != null)
            {
                throw new Exception("Este tipo de usu�rio n�o pode ser exclu�do pois est� associado a um ou mais usu�rios.");
            }

            _context.TipoUsuario.Remove(tipoUsuario);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<TipoUsuario>> GetAll()
        {
            var tipos = await _context.TipoUsuario.OrderBy(tu => tu.IdTipoUsuario).ToListAsync();
            if (tipos == null || !tipos.Any()) 
                throw new Exception("Nenhum tipo de usu�rio encontrado.");
            return tipos;
        }

        public async Task<TipoUsuario> GetById(int id)
        {
            var tipoUsuario = await _context.TipoUsuario.FirstOrDefaultAsync(tu => tu.IdTipoUsuario == id);
            if (tipoUsuario == null)
            {
                throw new KeyNotFoundException("Tipo de usu�rio n�o encontrado."); 
            }
            return tipoUsuario;
        }

        public async Task<TipoUsuario> UpdateById(int id, TipoUsuarioDto tipoUsuarioDto)
        {
            var tipoUsuario = await _context.TipoUsuario.FirstOrDefaultAsync(tu => tu.IdTipoUsuario == id);
            if (tipoUsuario == null)
            {
                throw new KeyNotFoundException("Tipo de usu�rio n�o encontrado."); 
            }

            if (tipoUsuario.DsTipoUsuario != tipoUsuarioDto.DsTipoUsuario)
            {
                var existingTipo = await _context.TipoUsuario.FirstOrDefaultAsync(tu => tu.DsTipoUsuario == tipoUsuarioDto.DsTipoUsuario && tu.IdTipoUsuario != id);
                if (existingTipo != null)
                {
                    throw new Exception("J� existe outro tipo de usu�rio com esta descri��o.");
                }
            }

            _mapper.Map(tipoUsuarioDto, tipoUsuario);
            await _context.SaveChangesAsync();
            return await GetById(id); // Return fetched entity
        }
    }
}