using GestordeTaras.EN;
using GestordeTareas.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestordeTareas.DAL
{
    public class CargoDAL: ICargoDAL
    {
        private readonly ContextoBD _dbContext;

        public CargoDAL(ContextoBD dbContext)
        {
            _dbContext = dbContext;
        }
        // Crear un nuevo Cargo
        public async Task<int> CreateAsync(Cargo cargo)
        {

            _dbContext.Add(cargo);
            return await _dbContext.SaveChangesAsync();
        }

        // Actualizar un Cargo existente
        public async Task<int> UpdateAsync(Cargo cargo)
        {
            var cargoBD = await _dbContext.Cargo.FirstOrDefaultAsync(c => c.Id == cargo.Id);

            if (cargoBD == null) return 0;

            cargoBD.Nombre = cargo.Nombre;
            _dbContext.Update(cargoBD);
            return await _dbContext.SaveChangesAsync();
        }

        // Eliminar un Cargo (verifica referencias)
        public async Task<int> DeleteAsync(Cargo cargo)
        {
            var cargoBD = await _dbContext.Cargo.FirstOrDefaultAsync(c => c.Id == cargo.Id);

            if (cargoBD == null) return 0;

            bool isAssociatedWithUsuario = await _dbContext.Usuario.AnyAsync(u => u.IdCargo == cargoBD.Id);
            if (isAssociatedWithUsuario)
                throw new Exception("No se puede eliminar el cargo porque está asociado con un usuario.");

            _dbContext.Cargo.Remove(cargoBD);
            return await _dbContext.SaveChangesAsync();
        }

        // Obtener un Cargo por ID
        public async Task<Cargo> GetByIdAsync(Cargo cargo)
        {
            return await _dbContext.Cargo.FirstOrDefaultAsync(c => c.Id == cargo.Id);
        }

        // Obtener todos los Cargos
        public async Task<List<Cargo>> GetAllAsync()
        {
            return await _dbContext.Cargo.ToListAsync();
        }

        // Obtener el ID del cargo "Colaborador"
        public async Task<int> GetCargoColaboradorIdAsync()
        {
            var cargoColaborador = await _dbContext.Cargo.FirstOrDefaultAsync(c => c.Nombre == "Colaborador");
            return cargoColaborador?.Id ?? 0;
        }
    }
}

