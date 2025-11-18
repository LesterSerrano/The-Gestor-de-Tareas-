using GestordeTaras.EN;
using GestordeTareas.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestordeTareas.DAL
{
    public class PrioridadDAL : IPrioridadDAL
    {
        private readonly ContextoBD _dbContext;

        public PrioridadDAL(ContextoBD dbContext)
        {
            _dbContext = dbContext;
        }
        //--------------------------------METODO CREAR PRIORIDAD--------------------------
        public async Task<int> CreateAsync(Prioridad prioridad)
        {

                _dbContext.Prioridad.Add(prioridad);
                return await _dbContext.SaveChangesAsync();

        }

        //--------------------------------METODO MODIFICAR PRIORIDAD--------------------------
        public async Task<int> UpdateAsync(Prioridad prioridad)
        {
            var prioridadDB = await _dbContext.Prioridad.FirstOrDefaultAsync(p => p.Id == prioridad.Id);

            if (prioridadDB == null)
                return 0;

            prioridadDB.Nombre = prioridad.Nombre;

            _dbContext.Prioridad.Update(prioridadDB);
            return await _dbContext.SaveChangesAsync();
        }

        //--------------------------------METODO ELIMINAR PRIORIDAD--------------------------
        public async Task<int> DeleteAsync(Prioridad prioridad)
        {
            var prioridadDB = await _dbContext.Prioridad.FirstOrDefaultAsync(p => p.Id == prioridad.Id);

            if (prioridadDB == null)
                return 0;

            _dbContext.Prioridad.Remove(prioridadDB);
            return await _dbContext.SaveChangesAsync();
        }

        //--------------------------METODO BUSCAR POR ID--------------------------------------------
        public async Task<Prioridad> GetByIdAsync(Prioridad prioridad)
        {
            return await _dbContext.Prioridad
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == prioridad.Id);
        }

        //--------------------------------METODO OBTENER TODAS LAS PRIORIDADES--------------------------
        public async Task<List<Prioridad>> GetAllAsync()
        {
            return await _dbContext.Prioridad
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
