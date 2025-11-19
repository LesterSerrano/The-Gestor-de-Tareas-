using GestordeTaras.EN;
using GestordeTareas.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestordeTareas.DAL
{
    public class EstadoTareaDAL: IEstadoTareaDAL
    {
        private readonly ContextoBD _dbContext;

        public EstadoTareaDAL(ContextoBD dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<int> CreateAsync(EstadoTarea estadoTarea)
        {
            await _dbContext.EstadoTarea.AddAsync(estadoTarea);
            return await _dbContext.SaveChangesAsync();
        }

        // Actualizar un estado de tarea
        public async Task<int> UpdateAsync(EstadoTarea estadoTarea)
        {
            var estadoTareaDB = await _dbContext.EstadoTarea.FirstOrDefaultAsync(e => e.Id == estadoTarea.Id);

            if (estadoTareaDB == null)
                return 0;

            estadoTareaDB.Nombre = estadoTarea.Nombre;
            _dbContext.EstadoTarea.Update(estadoTareaDB);
            return await _dbContext.SaveChangesAsync();
        }

        // Eliminar un estado de tarea
        public async Task<int> DeleteAsync(EstadoTarea estadoTarea)
        {
            var estadoTareaDB = await _dbContext.EstadoTarea.FirstOrDefaultAsync(e => e.Id == estadoTarea.Id);

            if (estadoTareaDB == null)
                return 0;

            _dbContext.EstadoTarea.Remove(estadoTareaDB);
            return await _dbContext.SaveChangesAsync();
        }

        // Obtener estado de tarea por ID
        public async Task<EstadoTarea> GetByIdAsync(EstadoTarea estadoTarea)
        {
            return await _dbContext.EstadoTarea.FirstOrDefaultAsync(e => e.Id == estadoTarea.Id);
        }

        // Obtener todos los estados
        public async Task<List<EstadoTarea>> GetAllAsync()
        {
            return await _dbContext.EstadoTarea.ToListAsync();
        }

        // Obtener ID del estado "Pendiente"
        public async Task<int> GetEstadoPendienteIdAsync()
        {
            var estadoPendiente = await _dbContext.EstadoTarea.FirstOrDefaultAsync(e => e.Nombre == "Pendiente");
            return estadoPendiente?.Id ?? 0;
        }
    }
}


