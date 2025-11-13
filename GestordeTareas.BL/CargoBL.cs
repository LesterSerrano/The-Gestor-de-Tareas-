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
    public class CargoBL
    {
        private readonly ICargoDAL ICargoDAL;

        public async Task<int> CreateAsync(Cargo cargo)
        {
            return await ICargoDAL.CreateAsync(cargo);
        }
        public async Task<int> UpdateAsync(Cargo cargo)
        {
            return await ICargoDAL.UpdateAsync(cargo);
        }
        public async Task<int> DeleteAsync(Cargo cargo)
        {
            return await ICargoDAL.DeleteAsync(cargo);
        }
        public async Task<Cargo> GetById(Cargo cargo)
        {
            return await ICargoDAL.GetByIdAsync(cargo);
        }
        public async Task<List<Cargo>> GetAllAsync()
        {
            return await ICargoDAL.GetAllAsync();
        }
    }
}
