using GestordeTaras.EN;
using GestordeTareas.BL.Services;
using GestordeTareas.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestordeTareas.BL
{
    public class UsuarioBL
    {
        private readonly IUsuarioDAL _usuarioDAL;
        private readonly ISeguridadService _seguridadService;

        public UsuarioBL(IUsuarioDAL usuarioDAL, ISeguridadService seguridadService)
        {
            _usuarioDAL = usuarioDAL;
            _seguridadService = seguridadService;
        }

        // ---------------- CRUD Usuario ----------------
        public async Task<int> Create(Usuario usuario)
        {
            usuario.Pass = _seguridadService.HashPassword(usuario.Pass);
            return await _usuarioDAL.CreateAsync(usuario);
        }

        public async Task<int> Update(Usuario usuario)
        {
            // Solo re-hashea la contraseña si fue modificada
            if (!string.IsNullOrEmpty(usuario.Pass))
                usuario.Pass = _seguridadService.HashPassword(usuario.Pass);

            return await _usuarioDAL.UpdateAsync(usuario);
        }

        public async Task<int> Delete(Usuario usuario)
        {
            return await _usuarioDAL.DeleteAsync(usuario);
        }

        public async Task<Usuario> GetByIdAsync(Usuario usuario)
        {
            return await _usuarioDAL.GetByIdAsync(usuario);
        }

        public async Task<Usuario> GetByNombreUsuarioAsync(Usuario usuario)
        {
            return await _usuarioDAL.GetByNombreUsuarioAsync(usuario);
        }

        public async Task<List<Usuario>> GetAllAsync()
        {
            return await _usuarioDAL.GetAllAsync();
        }

        public async Task<List<Usuario>> SearchAsync(Usuario usuario)
        {
            return await _usuarioDAL.SearchAsync(usuario);
        }

        public async Task<List<Usuario>> SearchIncludeRoleAsync(Usuario user, string query, string filter)
        {
            return await _usuarioDAL.SearchIncludeRoleAsync(user, query, filter);
        }

        // ---------------- Login ----------------
        public async Task<Usuario> LoginAsync(Usuario usuario)
        {
            var usuarioDb = await _usuarioDAL.GetByNombreUsuarioAsync(usuario);
            if (usuarioDb != null && _seguridadService.VerifyPassword(usuario.Pass, usuarioDb.Pass))
                return usuarioDb;

            return null;
        }

        // ---------------- Restablecimiento de contraseña ----------------
        public async Task<int> GenerarCodigoRestablecimientoAsync(Usuario usuario)
        {
            int codigo = new Random().Next(100000, 999999);
            string codigoHasheado = _seguridadService.HashPassword(codigo.ToString());

            var resetCode = new PasswordResetCode
            {
                IdUsuario = usuario.Id,
                Codigo = codigoHasheado,
                Expiration = DateTime.Now.AddMinutes(15)
            };

            return await _usuarioDAL.AddResetCodeAsync(resetCode);
        }

        public async Task<bool> ValidarCodigoRestablecimientoAsync(int idUsuario, string codigo)
        {
            string codigoHasheado = _seguridadService.HashPassword(codigo);
            var resetCode = await _usuarioDAL.GetResetCodeAsync(idUsuario, codigoHasheado);

            return resetCode != null && resetCode.Expiration >= DateTime.Now;
        }

        public async Task<int> RestablecerContrasenaAsync(int idUsuario, string codigo, string nuevaContrasena)
        {
            // Validación del código
            bool codigoValido = await ValidarCodigoRestablecimientoAsync(idUsuario, codigo);
            if (!codigoValido) throw new Exception("Código inválido o expirado");

            // Obtener usuario
            var usuario = await _usuarioDAL.GetByIdAsync(new Usuario { Id = idUsuario });
            usuario.Pass = _seguridadService.HashPassword(nuevaContrasena);

            // Actualizar contraseña
            int result = await _usuarioDAL.UpdateAsync(usuario);

            // Remover código de restablecimiento
            var resetCode = await _usuarioDAL.GetResetCodeAsync(idUsuario, _seguridadService.HashPassword(codigo));
            if (resetCode != null)
                await _usuarioDAL.RemoveResetCodeAsync(resetCode);

            return result;
        }
    }
}
