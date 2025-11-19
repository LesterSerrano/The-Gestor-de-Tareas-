using GestordeTaras.EN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestordeTareas.DAL.Interfaces
{
    public interface IProyectoUsuarioDAL
    {
        Task<int> UnirUsuarioAProyectoAsync(int idProyecto, int idUsuario);
        Task<List<Proyecto>> ObtenerProyectosPorUsuarioAsync(int idUsuario);
        Task<List<Usuario>> ObtenerUsuariosUnidosAsync(int idProyecto);
        Task<int> EliminarUsuarioDeProyectoAsync(int idProyecto, int idUsuario);
        Task<bool> AsignarEncargadoAsync(int idProyecto, int idUsuarioNuevoEncargado);
        Task<bool> IsUsuarioEncargadoAsync(int idProyecto, int idUsuario);
        Task<Usuario> ObtenerEncargadoPorProyectoAsync(int idProyecto);
        Task<List<ProyectoUsuario>> ObtenerTodosAsync();
    }
}
