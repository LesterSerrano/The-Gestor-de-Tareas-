using GestordeTaras.EN;
using GestordeTareas.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestordeTareas.DAL
{
    public class ProyectoUsuarioDAL : IProyectoUsuarioDAL
    {
        private readonly ContextoBD _dbContext;

        public ProyectoUsuarioDAL(ContextoBD dbContext)
        {
            _dbContext = dbContext;
        }
        // Método para unir un usuario a un proyecto
        public async Task<int> UnirUsuarioAProyectoAsync(int idProyecto, int idUsuario)
        {
            var proyectoUsuario = new ProyectoUsuario
            {
                IdProyecto = idProyecto,
                IdUsuario = idUsuario,
                FechaAsignacion = DateTime.Now
            };

            _dbContext.ProyectoUsuario.Add(proyectoUsuario);
            return await _dbContext.SaveChangesAsync();
        }

        // Método para obtener los proyectos a los que un usuario se ha unido
        public async Task<List<Proyecto>> ObtenerProyectosPorUsuarioAsync(int idUsuario)
        {
            return await _dbContext.ProyectoUsuario
                    .Where(pu => pu.IdUsuario == idUsuario)
                    .Include(pu => pu.Proyecto)
                    .Select(pu => pu.Proyecto)
                    .ToListAsync();
        }

        // Método para obtener los usuarios unidos a un proyecto
        public async Task<List<Usuario>> ObtenerUsuariosUnidosAsync(int idProyecto)
        {
            return await _dbContext.ProyectoUsuario
                    .Where(pu => pu.IdProyecto == idProyecto)
                    .Select(pu => pu.Usuario)
                    .ToListAsync();
        }

        // Método para eliminar la unión de un usuario con un proyecto
        public async Task<int> EliminarUsuarioDeProyectoAsync(int idProyecto, int idUsuario)
        {
                var proyectoUsuario = await _dbContext.ProyectoUsuario
                    .FirstOrDefaultAsync(pu => pu.IdProyecto == idProyecto && pu.IdUsuario == idUsuario);

                if (proyectoUsuario != null)
                {
                    _dbContext.ProyectoUsuario.Remove(proyectoUsuario);
                    return await _dbContext.SaveChangesAsync();
                }
                return 0;
        }

        // Método para asignar un usuario como encargado de un proyecto
        public async Task<bool> AsignarEncargadoAsync(int idProyecto, int idUsuarioNuevoEncargado)
        {
            var encargadoExistente = await _dbContext.ProyectoUsuario
                    .FirstOrDefaultAsync(pu => pu.IdProyecto == idProyecto && pu.Encargado);

            if (encargadoExistente != null)
                return false; // Ya existe un encargado

            var nuevoEncargado = await _dbContext.ProyectoUsuario
                .FirstOrDefaultAsync(pu => pu.IdProyecto == idProyecto && pu.IdUsuario == idUsuarioNuevoEncargado);

            if (nuevoEncargado != null)
            {
                nuevoEncargado.Encargado = true;
                await _dbContext.SaveChangesAsync();
                return true;
            }

            return false;

        }

        // Método para verificar si un usuario es encargado de un proyecto
        public async Task<bool> IsUsuarioEncargadoAsync(int idProyecto, int idUsuario)
        {
            return await _dbContext.ProyectoUsuario
                    .AnyAsync(pu => pu.IdProyecto == idProyecto && pu.IdUsuario == idUsuario && pu.Encargado);
        }

        // Método para obtener el encargado de un proyecto
        public async Task<Usuario> ObtenerEncargadoPorProyectoAsync(int idProyecto)
        {
            return await _dbContext.ProyectoUsuario
                    .Where(pu => pu.IdProyecto == idProyecto && pu.Encargado)
                    .Select(pu => pu.Usuario)
                    .FirstOrDefaultAsync();
        }

        // Método para obtener todos los registros de ProyectoUsuario
        public async Task<List<ProyectoUsuario>> ObtenerTodosAsync()
        {
            return await _dbContext.ProyectoUsuario
                    .Include(pu => pu.Proyecto)
                    .Include(pu => pu.Usuario)
                    .ToListAsync();
        }
 
    }
}
