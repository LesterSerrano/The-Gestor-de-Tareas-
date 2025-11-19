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
    public class EstadoTareaBL
    {
        private readonly IEstadoTareaDAL _EstadoTareaDAL;

        public EstadoTareaBL(IEstadoTareaDAL estadoTareaDAL)
        {
            _EstadoTareaDAL = estadoTareaDAL;
        }
        public async Task<int> CreateAsync(EstadoTarea estadoTarea)
        {
            return await _EstadoTareaDAL.CreateAsync(estadoTarea);
        }
        public async Task<int> UpdateAsync(EstadoTarea estadoTarea)
        {
            return await _EstadoTareaDAL.UpdateAsync(estadoTarea);
        }
        public async Task<int> DeleteAsync(EstadoTarea estadoTarea)
        {
            return await _EstadoTareaDAL.DeleteAsync(estadoTarea);
        }
        public async Task<EstadoTarea> GetByIdAsync(EstadoTarea estadoTarea)
        {
            return await _EstadoTareaDAL.GetByIdAsync(estadoTarea);
        }
        public async Task<List<EstadoTarea>> GetAllAsync()
        {
            return await _EstadoTareaDAL.GetAllAsync();
        }
        public async Task<int> GetEstadoPendienteIdAsync()
        {
            return await _EstadoTareaDAL.GetEstadoPendienteIdAsync();
        }
    }
}
