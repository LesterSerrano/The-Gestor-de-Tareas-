using GestordeTaras.EN;
using GestordeTareas.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestordeTareas.DAL.Interfaces;
using Microsoft.Extensions.Logging;

namespace GestordeTareas.DAL
{
    public class ProyectoDAL : IProyectoDAL
    {
        private readonly ContextoBD _dbContext;

        public ProyectoDAL(ContextoBD dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> CreateAsync(Proyecto proyecto)
        {
            await _dbContext.Proyecto.AddAsync(proyecto);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(Proyecto proyecto)
        {
            var proyectoDB = await _dbContext.Proyecto.FirstOrDefaultAsync(p => p.Id == proyecto.Id);

            if (proyectoDB == null)
                return 0;

            proyectoDB.Titulo = proyecto.Titulo;
            proyectoDB.Descripcion = proyecto.Descripcion;
            proyectoDB.IdUsuario = proyecto.IdUsuario;
            proyectoDB.FechaFinalizacion = proyecto.FechaFinalizacion;
            proyectoDB.CodigoAcceso = proyecto.CodigoAcceso;

            _dbContext.Proyecto.Update(proyectoDB);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(Proyecto proyecto)
        {
            var proyectoDB = await _dbContext.Proyecto.FirstOrDefaultAsync(p => p.Id == proyecto.Id);

            if (proyectoDB == null)
                return 0;

            _dbContext.Proyecto.Remove(proyectoDB);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<Proyecto> GetByIdAsync(Proyecto proyecto)
        {
            var proyectoDB = await _dbContext.Proyecto
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.Id == proyecto.Id);

            if (proyectoDB == null)
                throw new Exception("El proyecto no existe en la base de datos.");

            return proyectoDB;
        }

        public async Task<List<Proyecto>> GetAllAsync()
        {
            return await _dbContext.Proyecto
                .Include(p => p.Usuario)
                .Include(p => p.ProyectoUsuario)
                    .ThenInclude(pu => pu.Usuario)
                .ToListAsync();
        }

        public async Task<bool> ExisteCodigoAccesoAsync(string codigoAcceso)
        {
            return await _dbContext.Proyecto.AnyAsync(p => p.CodigoAcceso == codigoAcceso);
        }

        public async Task<List<Proyecto>> BuscarPorTituloOAdministradorAsync(string query)
        {
            return await _dbContext.Proyecto
                .Include(p => p.Usuario)
                .Where(p => p.Titulo.Contains(query) || p.Usuario.Nombre.Contains(query))
                .ToListAsync();
        }
    }
}
