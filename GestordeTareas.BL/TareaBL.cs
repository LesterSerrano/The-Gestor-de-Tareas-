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
    public class TareaBL
    {
        private readonly ITareaDAL _TareaDAL;

        public TareaBL(ITareaDAL tareaDAL)
        {
            _TareaDAL = tareaDAL;
        }
        public async Task<int> CreateAsync(Tarea tarea)
        {
            return await _TareaDAL.CreateAsync(tarea);
        }
        public async Task<int> UpdateAsync(Tarea tarea)
        {
            return await _TareaDAL.UpdateAsync(tarea);
        }
        public async Task<int> DeleteAsync(Tarea tarea)
        {
            return await _TareaDAL.DeleteAsync(tarea);
        }
        public async Task<Tarea> GetByIdAsync(Tarea tarea)
        {
            return await _TareaDAL.GetByIdAsync(tarea);
        }
        public async Task<List<Tarea>> GetAllAsync()
        {
            return await _TareaDAL.GetAllAsync();
        }
        public async Task<List<Tarea>> GetTareasByProyectoIdAsync(int proyectoId)
        {
            return await _TareaDAL.GetTareasByProyectoIdAsync(proyectoId);
        }

        public async Task<int> ActualizarEstadoTareaAsync(int idTarea, int idEstadoTarea)
        {
            return await _TareaDAL.ActualizarEstadoTareaAsync(idTarea, idEstadoTarea);
        }

        public async Task<(bool success, string mensaje, string nombreEstado)> ActualizarEstadoTareaConValidacionAsync(int idTarea, int idEstadoTarea)
        {
            // Obtener la tarea
            var tarea = await _TareaDAL.GetByIdAsync(new Tarea { Id = idTarea });
            if (tarea == null) return (false, "Tarea no encontrada", null);

            // Obtener y validar el estado
            var estado = await _TareaDAL.GetEstadoByIdAsync(idEstadoTarea);
            if (estado == null) return (false, "Estado no válido", null);

            // Actualizar el estado
            await _TareaDAL.ActualizarEstadoTareaAsync(idTarea, idEstadoTarea);

            return (true, "Estado actualizado", estado.Nombre);
        }


    }
}