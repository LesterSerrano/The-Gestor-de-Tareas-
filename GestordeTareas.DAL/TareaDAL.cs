using GestordeTaras.EN;
using GestordeTareas.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GestordeTareas.DAL
{
    public class TareaDAL : ITareaDAL
    {
        private readonly ContextoBD _dbContext;

        public TareaDAL(ContextoBD dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<int> CreateAsync(Tarea tarea)
        {
            await _dbContext.Tarea.AddAsync(tarea);
            return await _dbContext.SaveChangesAsync();
        }

        // Actualizar una tarea
        public async Task<int> UpdateAsync(Tarea tarea)
        {
            var tareaDB = await _dbContext.Tarea.FirstOrDefaultAsync(t => t.Id == tarea.Id);

            if (tareaDB == null)
                return 0;

            tareaDB.Nombre = tarea.Nombre;
            tareaDB.Descripcion = tarea.Descripcion;
            tareaDB.FechaCreacion = tarea.FechaCreacion;
            tareaDB.FechaVencimiento = tarea.FechaVencimiento;
            tareaDB.IdCategoria = tarea.IdCategoria;
            tareaDB.IdPrioridad = tarea.IdPrioridad;
            tareaDB.IdEstadoTarea = tarea.IdEstadoTarea;
            tareaDB.IdProyecto = tarea.IdProyecto;

            _dbContext.Tarea.Update(tareaDB);
            return await _dbContext.SaveChangesAsync();
        }

        // Eliminar una tarea
        public async Task<int> DeleteAsync(Tarea tarea)
        {
            var tareaDB = await _dbContext.Tarea.FirstOrDefaultAsync(t => t.Id == tarea.Id);

            if (tareaDB == null)
                return 0;

            _dbContext.Tarea.Remove(tareaDB);
            return await _dbContext.SaveChangesAsync();
        }

        // Obtener tarea por ID
        public async Task<Tarea> GetByIdAsync(Tarea tarea)
        {
            return await _dbContext.Tarea
                .Include(c => c.Categoria)
                .Include(p => p.Prioridad)
                .Include(e => e.EstadoTarea)
                .Include(r => r.Proyecto)
                .FirstOrDefaultAsync(t => t.Id == tarea.Id);
        }

        // Obtener todas las tareas
        public async Task<List<Tarea>> GetAllAsync()
        {
            return await _dbContext.Tarea
                .Include(c => c.Categoria)
                .Include(p => p.Prioridad)
                .Include(e => e.EstadoTarea)
                .Include(r => r.Proyecto)
                .ToListAsync();
        }

        // Obtener tareas por ID de proyecto
        public async Task<List<Tarea>> GetTareasByProyectoIdAsync(int proyectoId)
        {
            try
            {
                var tareas = await _dbContext.Tarea
                    .Include(c => c.Categoria)
                    .Include(p => p.Prioridad)
                    .Include(e => e.EstadoTarea)
                    .Include(r => r.Proyecto)
                    .Where(t => t.IdProyecto == proyectoId)
                    .ToListAsync();

                Debug.WriteLine($"Tareas encontradas: {tareas.Count}");
                return tareas;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en GetTareasByProyectoIdAsync: {ex.Message}");
                return new List<Tarea>();
            }
        }

        // Actualizar el estado de una tarea
        public async Task<int> ActualizarEstadoTareaAsync(int idTarea, int idEstadoTarea)
        {
            var tareaDB = await _dbContext.Tarea.FirstOrDefaultAsync(t => t.Id == idTarea);

            if (tareaDB == null)
                return 0;

            tareaDB.IdEstadoTarea = idEstadoTarea;
            _dbContext.Tarea.Update(tareaDB);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<EstadoTarea> GetEstadoByIdAsync(int idEstadoTarea)
        {
            return await _dbContext.EstadoTarea.FirstOrDefaultAsync(e => e.Id == idEstadoTarea);
        }

    }
}


