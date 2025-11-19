using GestordeTaras.EN;
using GestordeTareas.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GestordeTareas.DAL
{
    public class UsuarioDAL : IUsuarioDAL
    {
        private readonly ContextoBD _dbContext;

        public UsuarioDAL(ContextoBD dbContext)
        {
            _dbContext = dbContext;
        }

        // ---------------- CRUD Usuario ----------------
        public async Task<int> CreateAsync(Usuario usuario)
        {
            usuario.FechaRegistro = DateTime.Now;
            _dbContext.Usuario.Add(usuario);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(Usuario usuario)
        {
            var userDb = await _dbContext.Usuario.FirstOrDefaultAsync(u => u.Id == usuario.Id);
            if (userDb == null) throw new Exception("Usuario no encontrado.");

            userDb.Nombre = usuario.Nombre;
            userDb.Apellido = usuario.Apellido;
            userDb.Telefono = usuario.Telefono;
            userDb.Pass = usuario.Pass;
            userDb.Status = usuario.Status;
            userDb.NombreUsuario = usuario.NombreUsuario;

            _dbContext.Usuario.Update(userDb);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(Usuario usuario)
        {
            var userDb = await _dbContext.Usuario.FirstOrDefaultAsync(u => u.Id == usuario.Id);
            if (userDb == null) return 0;

            _dbContext.Usuario.Remove(userDb);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<Usuario> GetByIdAsync(Usuario usuario)
        {
            return await _dbContext.Usuario.FirstOrDefaultAsync(u => u.Id == usuario.Id);
        }

        public async Task<Usuario> GetByNombreUsuarioAsync(Usuario usuario)
        {
            return await _dbContext.Usuario.FirstOrDefaultAsync(u => u.NombreUsuario == usuario.NombreUsuario);
        }

        public async Task<List<Usuario>> GetAllAsync()
        {
            return await _dbContext.Usuario.ToListAsync();
        }

        public async Task<List<Usuario>> SearchAsync(Usuario usuario)
        {
            var query = _dbContext.Usuario.AsQueryable();

            if (!string.IsNullOrEmpty(usuario.Nombre))
                query = query.Where(u => u.Nombre.Contains(usuario.Nombre));

            if (!string.IsNullOrEmpty(usuario.Apellido))
                query = query.Where(u => u.Apellido.Contains(usuario.Apellido));

            return await query.ToListAsync();
        }

        public async Task<List<Usuario>> SearchIncludeRoleAsync(Usuario user, string query, string filter)
        {
            var select = _dbContext.Usuario.AsQueryable();

            if (!string.IsNullOrEmpty(query))
            {
                if (filter == "Apellido")
                    select = select.Where(u => u.Apellido.Contains(query));
                else if (filter == "NombreUsuario")
                    select = select.Where(u => u.NombreUsuario.Contains(query));
            }

            return await select.ToListAsync();
        }

        //public async Task<Usuario> LoginAsync(Usuario usuario)
        //{
        //    return await _dbContext.Usuario.FirstOrDefaultAsync(u =>
        //        u.NombreUsuario == usuario.NombreUsuario &&
        //        u.Pass == usuario.Pass &&
        //        u.Status == (byte)User_Status.ACTIVO);
        //}

        // ---------------- Métodos de restablecimiento de contraseña ----------------
        public async Task<int> AddResetCodeAsync(PasswordResetCode resetCode)
        {
            _dbContext.PasswordResetCode.Add(resetCode);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<PasswordResetCode> GetResetCodeAsync(int idUsuario, string codigo)
        {
            return await _dbContext.PasswordResetCode
                .FirstOrDefaultAsync(c => c.IdUsuario == idUsuario && c.Codigo == codigo);
        }

        public async Task<int> RemoveResetCodeAsync(PasswordResetCode resetCode)
        {
            _dbContext.PasswordResetCode.Remove(resetCode);
            return await _dbContext.SaveChangesAsync();
        }

        // ---------------- Métodos internos ----------------
        public async Task<string> TieneReferenciasAsync(int usuarioId)
        {
            bool tieneProyectos = await _dbContext.ProyectoUsuario.AnyAsync(p => p.IdUsuario == usuarioId);
            if (tieneProyectos) return "Este usuario está asociado a un proyecto.";
            return null;
        }
    }
}
