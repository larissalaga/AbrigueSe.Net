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
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UsuarioRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Usuario> Create(UsuarioCreateDto usuarioDto)
        {
            var tipoUsuarioExists = await _context.TipoUsuario.FirstOrDefaultAsync(tu => tu.IdTipoUsuario == usuarioDto.IdTipoUsuario);
            if (tipoUsuarioExists == null)
                throw new Exception("Tipo de usuário não encontrado.");

            var pessoaExists = await _context.Pessoa.FirstOrDefaultAsync(p => p.IdPessoa == usuarioDto.IdPessoa);
            if (pessoaExists == null)
                throw new Exception("Pessoa não encontrada.");

            var userByNameExists = await _context.Usuario.FirstOrDefaultAsync(u => u.NmUsuario == usuarioDto.NmUsuario);
            if (userByNameExists != null)
                throw new Exception("Nome de usuário já existe.");

            var userByEmailExists = await _context.Usuario.FirstOrDefaultAsync(u => u.DsEmail == usuarioDto.DsEmail);
            if (userByEmailExists != null)
                throw new Exception("Email já cadastrado.");

            var userByPessoaExists = await _context.Usuario.FirstOrDefaultAsync(u => u.IdPessoa == usuarioDto.IdPessoa);
            if (userByPessoaExists != null)
                throw new Exception("Já existe um usuário para esta pessoa.");

            var nextId = await _context.GetNextSequenceValueAsync("seq_t_gsab_usuario"); 
            var newUsuario = _mapper.Map<Usuario>(usuarioDto);
            newUsuario.IdUsuario = nextId;
            // Password hashing should be handled in a service layer or here if no service layer
            // newUsuario.DsSenha = HashPassword(usuarioDto.DsSenha); 

            _context.Usuario.Add(newUsuario);
            await _context.SaveChangesAsync();
            return await GetById(newUsuario.IdUsuario); 
        }

        public async Task<bool> DeleteById(int id)
        {
            var usuario = await _context.Usuario.FirstOrDefaultAsync(u => u.IdUsuario == id);
            if (usuario == null)
            {
                return false;
            }

            _context.Usuario.Remove(usuario);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Usuario>> GetAll()
        {
            return await _context.Usuario
                                 .Include(u => u.Pessoa)
                                 .Include(u => u.TipoUsuario)
                                 .OrderBy(u => u.IdUsuario)
                                 .ToListAsync();
        }

        public async Task<Usuario> GetByEmail(string email)
        {
            var usuario = await _context.Usuario
                                        .Include(u => u.Pessoa)
                                        .Include(u => u.TipoUsuario)
                                        .FirstOrDefaultAsync(u => u.DsEmail == email);
            // Não lançar exceção aqui, pois pode ser usado para verificar existência
            return usuario;
        }

        public async Task<Usuario> GetById(int id)
        {
            var usuario = await _context.Usuario
                                        .Include(u => u.Pessoa)
                                        .Include(u => u.TipoUsuario)
                                        .FirstOrDefaultAsync(x => x.IdUsuario == id);
            if (usuario == null)
            {
                throw new Exception("Usuário não encontrado.");
            }
            return usuario;
        }

        public async Task<Usuario> UpdateById(int id, UsuarioUpdateDto usuarioDto)
        {
            var usuario = await _context.Usuario.FirstOrDefaultAsync(u => u.IdUsuario == id);
            if (usuario == null)
            {
                throw new Exception("Usuário não encontrado.");
            }

            if (usuarioDto.IdTipoUsuario.HasValue && usuario.IdTipoUsuario != usuarioDto.IdTipoUsuario.Value)
            {
                var tipoUsuarioExists = await _context.TipoUsuario.FirstOrDefaultAsync(tu => tu.IdTipoUsuario == usuarioDto.IdTipoUsuario.Value);
                if (tipoUsuarioExists == null)
                    throw new Exception("Novo tipo de usuário não encontrado.");
            }

            if (!string.IsNullOrEmpty(usuarioDto.NmUsuario) && usuario.NmUsuario != usuarioDto.NmUsuario)
            {
                var userByNameExists = await _context.Usuario.FirstOrDefaultAsync(u => u.NmUsuario == usuarioDto.NmUsuario && u.IdUsuario != id);
                if (userByNameExists != null)
                    throw new Exception("Nome de usuário já existe.");
            }

            if (!string.IsNullOrEmpty(usuarioDto.DsEmail) && usuario.DsEmail != usuarioDto.DsEmail)
            {
                var userByEmailExists = await _context.Usuario.FirstOrDefaultAsync(u => u.DsEmail == usuarioDto.DsEmail && u.IdUsuario != id);
                if (userByEmailExists != null)
                    throw new Exception("Email já cadastrado.");
            }

            _mapper.Map(usuarioDto, usuario);
            // Password hashing for usuarioDto.DsSenha if it's not empty and part of update DTO
            // if (!string.IsNullOrEmpty(usuarioDto.DsSenha)) 
            //    usuario.DsSenha = HashPassword(usuarioDto.DsSenha);

            await _context.SaveChangesAsync();
            return await GetById(id); 
        }

        public async Task<Usuario> GetByLogin(string login)
        {
            var usuario = await _context.Usuario
                                        .Include(u => u.Pessoa)
                                        .Include(u => u.TipoUsuario)
                                        .FirstOrDefaultAsync(u => u.NmUsuario == login || u.DsEmail == login);
            if (usuario == null)
            {
                throw new Exception("Usuário não encontrado.");
            }
            return usuario;
        }
    }
}