using GestordeTaras.EN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestordeTareas.DAL.Interfaces
{
    public interface ICargoDAL
    {
        Task<int> CreateAsync(Cargo cargo);
        Task<int> UpdateAsync(Cargo cargo);
        Task<int> DeleteAsync(Cargo cargo);
        Task<Cargo> GetByIdAsync(Cargo cargo);
        Task<List<Cargo>> GetAllAsync();
        Task<int> GetCargoColaboradorIdAsync();

    }
}
