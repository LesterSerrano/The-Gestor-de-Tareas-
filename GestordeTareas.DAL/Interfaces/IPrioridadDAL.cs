using GestordeTaras.EN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestordeTareas.DAL.Interfaces
{
    public interface IPrioridadDAL
    {
        Task<int> CreateAsync(Prioridad prioridad);
        Task<int> UpdateAsync(Prioridad prioridad);
        Task<int> DeleteAsync(Prioridad prioridad);
        Task<Prioridad> GetByIdAsync(Prioridad prioridad);
        Task<List<Prioridad>> GetAllAsync();
    }
}
