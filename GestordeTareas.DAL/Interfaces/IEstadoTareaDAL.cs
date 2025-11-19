using GestordeTaras.EN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestordeTareas.DAL.Interfaces
{
    public interface IEstadoTareaDAL
    {
        Task<int> CreateAsync(EstadoTarea estadoTarea);
        Task<int> UpdateAsync(EstadoTarea estadoTarea);
        Task<int> DeleteAsync(EstadoTarea estadoTarea);
        Task<EstadoTarea> GetByIdAsync(EstadoTarea estadoTarea);
        Task<List<EstadoTarea>> GetAllAsync();
        Task<int> GetEstadoPendienteIdAsync();
    }
}
