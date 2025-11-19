using GestordeTaras.EN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestordeTareas.DAL.Interfaces
{
    public interface ITareaDAL
    {
        Task<int> CreateAsync(Tarea tarea);
        Task<int> UpdateAsync(Tarea tarea);
        Task<int> DeleteAsync(Tarea tarea);
        Task<Tarea> GetByIdAsync(Tarea tarea);
        Task<List<Tarea>> GetAllAsync();
        Task<List<Tarea>> GetTareasByProyectoIdAsync(int proyectoId);
        Task<int> ActualizarEstadoTareaAsync(int idTarea, int idEstadoTarea);
        Task<EstadoTarea> GetEstadoByIdAsync(int idEstadoTarea);

    }
}
