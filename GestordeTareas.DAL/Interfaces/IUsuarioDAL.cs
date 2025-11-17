using GestordeTaras.EN;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestordeTareas.DAL.Interfaces
{
    public interface IUsuarioDAL
    {
        Task<int> CreateAsync(Usuario usuario);
        Task<int> UpdateAsync(Usuario usuario);
        Task<int> DeleteAsync(Usuario usuario);
        Task<Usuario> GetByIdAsync(Usuario usuario);
        Task<Usuario> GetByNombreUsuarioAsync(Usuario usuario);
        Task<List<Usuario>> GetAllAsync();
        Task<List<Usuario>> SearchAsync(Usuario usuario);
        Task<List<Usuario>> SearchIncludeRoleAsync(Usuario user, string query, string filter);
        Task<Usuario> LoginAsync(Usuario usuario);

        // Métodos de restablecimiento de contraseña
        Task<int> AddResetCodeAsync(PasswordResetCode resetCode);
        Task<PasswordResetCode> GetResetCodeAsync(int idUsuario, string codigo);
        Task<int> RemoveResetCodeAsync(PasswordResetCode resetCode);

        // Métodos internos que la BL no necesita exponer (se quedan en la clase)
        Task<string> TieneReferenciasAsync(int usuarioId);
    }
}
