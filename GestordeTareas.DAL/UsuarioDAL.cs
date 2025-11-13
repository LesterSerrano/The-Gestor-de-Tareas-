using GestordeTaras.EN;
using Microsoft.EntityFrameworkCore;
using GestordeTareas.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GestordeTareas.DAL
{
    public class UsuarioDAL: IUsuarioDAL
    {
        private readonly ContextoBD _dbContext;

        public UsuarioDAL(ContextoBD dbContext)
        {
            _dbContext = dbContext;
        }
        // Genera un hash MD5 de una cadena de texto
        public static string HashMD5(string input)
        {
            using var md5 = MD5.Create();
            var hashBytes = md5.ComputeHash(Encoding.ASCII.GetBytes(input));
            return string.Concat(hashBytes.Select(b => b.ToString("x2").ToLower()));
        }

        // Encripta la contraseña del usuario usando MD5
        private void EncryptMD5(Usuario user)
        {
            user.Pass = HashMD5(user.Pass);
        }

        // Verifica si el nombre de usuario o teléfono ya existen
        private async Task<bool> ExistsLoginAsync(Usuario user)
        {
            bool loginExists = await _dbContext.Usuario.AnyAsync(u =>
                (u.NombreUsuario == user.NombreUsuario || u.Telefono == user.Telefono) && u.Id != user.Id);
            return loginExists;
        }

        // Crear un nuevo usuario
        public async Task<int> CreateAsync(Usuario usuario)
        {
            bool exists = await ExistsLoginAsync(usuario);
            if (exists)
                throw new Exception("El correo electrónico o número de teléfono ya pertenece a un usuario.");

            usuario.FechaRegistro = DateTime.Now;
            EncryptMD5(usuario);

            _dbContext.Usuario.Add(usuario);
            return await _dbContext.SaveChangesAsync();
        }

        // Actualizar un usuario existente
        public async Task<int> UpdateAsync(Usuario usuario)
        {
            if (await ExistsLoginAsync(usuario))
                throw new Exception("El correo electrónico o teléfono ya está registrado.");

            var userDb = await _dbContext.Usuario.FirstOrDefaultAsync(u => u.Id == usuario.Id);
            if (userDb == null) throw new Exception("Usuario no encontrado.");

            userDb.IdCargo = usuario.IdCargo;
            userDb.Nombre = usuario.Nombre;
            userDb.Apellido = usuario.Apellido;
            userDb.Status = usuario.Status;
            userDb.Telefono = usuario.Telefono;
            userDb.FechaNacimiento = usuario.FechaNacimiento;
            userDb.NombreUsuario = usuario.NombreUsuario;

            if (!string.IsNullOrEmpty(usuario.FotoPerfil))
                userDb.FotoPerfil = usuario.FotoPerfil;

            if (!string.IsNullOrEmpty(usuario.Pass) && usuario.Pass != userDb.Pass)
            {
                EncryptMD5(usuario);
                userDb.Pass = usuario.Pass;
            }

            _dbContext.Usuario.Update(userDb);
            return await _dbContext.SaveChangesAsync();
        }

        // Eliminar un usuario
        public async Task<int> DeleteAsync(Usuario usuario)
        {
            string mensajeReferencia = await TieneReferenciasAsync(usuario.Id);
            if (mensajeReferencia != null)
                throw new Exception(mensajeReferencia);

            var usuarioDB = await _dbContext.Usuario.FirstOrDefaultAsync(u => u.Id == usuario.Id);
            if (usuarioDB == null) return 0;

            _dbContext.Usuario.Remove(usuarioDB);
            return await _dbContext.SaveChangesAsync();
        }

        // Obtener un usuario por ID
        public async Task<Usuario> GetByIdAsync(Usuario usuario)
        {
            return await _dbContext.Usuario
                .Include(u => u.Cargo)
                .FirstOrDefaultAsync(u => u.Id == usuario.Id);
        }

        // Obtener un usuario por nombre de usuario
        public async Task<Usuario> GetByNombreUsuarioAsync(Usuario usuario)
        {
            return await _dbContext.Usuario
                .Include(u => u.Cargo)
                .FirstOrDefaultAsync(u => u.NombreUsuario == usuario.NombreUsuario);
        }

        // Obtener todos los usuarios
        public async Task<List<Usuario>> GetAllAsync()
        {
            return await _dbContext.Usuario
                .Include(c => c.Cargo)
                .ToListAsync();
        }

        // Filtro dinámico
        internal IQueryable<Usuario> QuerySelect(IQueryable<Usuario> query, Usuario user)
        {
            if (user.Id > 0)
                query = query.Where(u => u.Id == user.Id);
            if (user.IdCargo > 0)
                query = query.Where(u => u.IdCargo == user.IdCargo);
            if (!string.IsNullOrWhiteSpace(user.Nombre))
                query = query.Where(u => u.Nombre.Contains(user.Nombre));
            if (!string.IsNullOrWhiteSpace(user.Apellido))
                query = query.Where(u => u.Apellido.Contains(user.Apellido));
            if (!string.IsNullOrWhiteSpace(user.NombreUsuario))
                query = query.Where(u => u.NombreUsuario.Contains(user.NombreUsuario));
            if (user.Status > 0)
                query = query.Where(u => u.Status == user.Status);

            if (user.FechaRegistro.Year > 1000)
            {
                DateTime ini = user.FechaRegistro.Date;
                DateTime fin = ini.AddDays(1).AddMilliseconds(-1);
                query = query.Where(u => u.FechaRegistro >= ini && u.FechaRegistro <= fin);
            }

            query = query.OrderByDescending(u => u.Id);
            if (user.Top_Aux > 0)
                query = query.Take(user.Top_Aux);

            return query;
        }

        // Búsqueda genérica
        public async Task<List<Usuario>> SearchAsync(Usuario usuario)
        {
            var select = QuerySelect(_dbContext.Usuario.AsQueryable(), usuario)
                .Include(u => u.Cargo);
            return await select.ToListAsync();
        }

        // Búsqueda con filtro y rol
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

            select = QuerySelect(select, user).Include(u => u.Cargo);
            return await select.ToListAsync();
        }

        // Login de usuario
        public async Task<Usuario> LoginAsync(Usuario usuario)
        {
            EncryptMD5(usuario);

            return await _dbContext.Usuario.FirstOrDefaultAsync(u =>
                u.NombreUsuario == usuario.NombreUsuario &&
                u.Pass == usuario.Pass &&
                u.Status == (byte)User_Status.ACTIVO);
        }

        // Verifica si un usuario tiene referencias en otras tablas
        public async Task<string> TieneReferenciasAsync(int usuarioId)
        {
            bool tieneProyectos = await _dbContext.ProyectoUsuario.AnyAsync(pu => pu.IdUsuario == usuarioId);
            if (tieneProyectos)
                return "Este usuario está asociado a un proyecto.";

            return null;
        }

        public async Task<int> GenerarCodigoRestablecimientoAsync(Usuario usuario)
        {
            int codigo = new Random().Next(100000, 999999);
            string codigoEncriptado = HashMD5(codigo.ToString());

            var resetCode = new PasswordResetCode
            {
                IdUsuario = usuario.Id,
                Codigo = codigoEncriptado,
                Expiration = DateTime.Now.AddMinutes(15)
            };

            _dbContext.PasswordResetCode.Add(resetCode);
            return await _dbContext.SaveChangesAsync();
        }


        // Validar código de restablecimiento
        public async Task<bool> ValidarCodigoRestablecimientoAsync(int idUsuario, string codigo)
        {
            string codigoHash = HashMD5(codigo);

            var resetCode = await _dbContext.PasswordResetCode
                .FirstOrDefaultAsync(c => c.IdUsuario == idUsuario && c.Codigo == codigoHash);

            return resetCode != null && resetCode.Expiration >= DateTime.Now;
        }

        // Restablecer contraseña
        public async Task<int> RestablecerContrasenaAsync(int idUsuario, string codigo, string nuevaContrasena)
        {
            string codigoHash = HashMD5(codigo);

            var resetCode = await _dbContext.PasswordResetCode
                .FirstOrDefaultAsync(c => c.IdUsuario == idUsuario && c.Codigo == codigoHash);

            if (resetCode == null || resetCode.Expiration < DateTime.Now)
                throw new Exception("Token inválido o expirado.");

            var usuario = await _dbContext.Usuario.FindAsync(idUsuario);
            usuario.Pass = HashMD5(nuevaContrasena);

            _dbContext.PasswordResetCode.Remove(resetCode);
            _dbContext.Usuario.Update(usuario);
            return await _dbContext.SaveChangesAsync();
        }
    }
}