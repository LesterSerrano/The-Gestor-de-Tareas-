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
        private readonly ICargoDAL _CargoDAL;

        public CargoBL(ICargoDAL cargoDAL)
        {
            _CargoDAL = cargoDAL;
        }

        public async Task<int> CreateAsync(Cargo cargo)
        {
            return await _CargoDAL.CreateAsync(cargo);
        }
        public async Task<int> UpdateAsync(Cargo cargo)
        {
            return await _CargoDAL.UpdateAsync(cargo);
        }
        public async Task<int> DeleteAsync(Cargo cargo)
        {
            return await _CargoDAL.DeleteAsync(cargo);
        }
        public async Task<Cargo> GetById(Cargo cargo)
        {
            return await _CargoDAL.GetByIdAsync(cargo);
        }
        public async Task<List<Cargo>> GetAllAsync()
        {
            return await _CargoDAL.GetAllAsync();
        }
        public async Task<int> GetCargoColaboradorIdAsync()
        {
            return await _CargoDAL.GetCargoColaboradorIdAsync();
        }

    }
}
