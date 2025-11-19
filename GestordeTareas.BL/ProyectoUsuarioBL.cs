using GestordeTaras.EN;
using GestordeTareas.DAL;
using GestordeTareas.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestordeTareas.BL
{
    public class ProyectoUsuarioBL
    {
        private readonly IProyectoUsuarioDAL _proyectoUsuarioDAL;

        public ProyectoUsuarioBL(IProyectoUsuarioDAL proyectoUsuarioDAL)
        {
            _proyectoUsuarioDAL = proyectoUsuarioDAL;
        }
        public async Task<int> UnirUsuarioAProyectoAsync(int idProyecto, int idUsuario)
        {
            // Llama al método en la capa DAL
            return await _proyectoUsuarioDAL.UnirUsuarioAProyectoAsync(idProyecto, idUsuario);
        }

        public async Task<List<Proyecto>> ObtenerProyectosPorUsuarioAsync(int idUsuario)
        {
            // Llama al método en la capa DAL
            return await _proyectoUsuarioDAL.ObtenerProyectosPorUsuarioAsync(idUsuario);
        }

        public async Task<List<Usuario>> ObtenerUsuariosUnidosAsync(int idProyecto)
        {
            return await _proyectoUsuarioDAL.ObtenerUsuariosUnidosAsync(idProyecto);
        }

        public async Task<int> EliminarUsuarioDeProyectoAsync(int idProyecto, int idUsuario)
        {
            // Llamar al método de ProyectoDAL para eliminar la relación
            return await _proyectoUsuarioDAL.EliminarUsuarioDeProyectoAsync(idProyecto, idUsuario);
        }

        // Método para asignar un usuario como encargado de un proyecto
        public async Task<bool> AsignarEncargadoAsync(int idProyecto, int idUsuarioNuevoEncargado)
        {
            // Llama al método en la capa DAL
            return await _proyectoUsuarioDAL.AsignarEncargadoAsync(idProyecto, idUsuarioNuevoEncargado);
        }

        // Método para verificar si un usuario es el encargado de un proyecto
        public async Task<bool> IsUsuarioEncargadoAsync(int idProyecto, int idUsuario)
        {
            return await _proyectoUsuarioDAL.IsUsuarioEncargadoAsync(idProyecto, idUsuario);
        }

        // Método para obtener el encargado de un proyecto
        public async Task<Usuario> ObtenerEncargadoPorProyectoAsync(int idProyecto)
        {
            return await _proyectoUsuarioDAL.ObtenerEncargadoPorProyectoAsync(idProyecto);
        }

        public async Task<List<ProyectoUsuario>> ObtenerTodosAsync()
        {
            return await _proyectoUsuarioDAL.ObtenerTodosAsync();
        }


    }
}
