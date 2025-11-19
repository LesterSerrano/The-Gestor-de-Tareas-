using GestordeTaras.EN;
<<<<<<< HEAD
=======
using GestordeTareas.DAL;
>>>>>>> 6f3904e1ef769397f15b3f638d17e031b23152fb
using GestordeTareas.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestordeTareas.BL
{
    public class PrioridadBL
    {
<<<<<<< HEAD
        private readonly IPrioridad _prioridadDAL;

        public PrioridadBL(IPrioridad prioridadDAL)
        {
            _prioridadDAL = prioridadDAL;
=======
        private IPrioridadDAL _prioridadDAL;

        public PrioridadBL(IPrioridadDAL prioridadDAL)
        {
            _prioridadDAL = prioridadDAL;
        }
        public async Task<int> CreateAsync(Prioridad prioridad)
        {
            return await _prioridadDAL.CreateAsync(prioridad);
>>>>>>> 6f3904e1ef769397f15b3f638d17e031b23152fb
        }

        public async Task<Prioridad> CreateAsync(Prioridad prioridad)
        {
<<<<<<< HEAD
            return await _prioridadDAL.CreatePrioridadAsync(prioridad);
=======
            return await _prioridadDAL.UpdateAsync(prioridad);
>>>>>>> 6f3904e1ef769397f15b3f638d17e031b23152fb
        }

        public async Task<Prioridad> UpdateAsync(Prioridad prioridad)
        {
<<<<<<< HEAD
            return await _prioridadDAL.UpdatePrioridadAsync(prioridad);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _prioridadDAL.DeletePrioridadAsync(id);
=======
            return await _prioridadDAL.DeleteAsync(prioridad);
        }
        public async Task<Prioridad> GetByIdAsync(Prioridad prioridad)
        {
            return await _prioridadDAL.GetByIdAsync(prioridad);
>>>>>>> 6f3904e1ef769397f15b3f638d17e031b23152fb
        }

        public async Task<Prioridad> GetByIdAsync(int id)
        {
<<<<<<< HEAD
            return await _prioridadDAL.GetPrioridadByIdAsync(id);
        }

        public async Task<IEnumerable<Prioridad>> GetAllAsync()
        {
            return await _prioridadDAL.GetAllPrioridadesAsync();
=======
            return await _prioridadDAL.GetAllAsync();
>>>>>>> 6f3904e1ef769397f15b3f638d17e031b23152fb
        }
    }
}
