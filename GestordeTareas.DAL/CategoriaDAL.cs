using GestordeTaras.EN;
using GestordeTareas.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace GestordeTareas.DAL
{
    public class CategoriaDAL : ICategoriaDAL
    {
        private readonly ContextoBD _dbContext;

        public CategoriaDAL(ContextoBD dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<int> CreateAsync(Categoria categoria)
        {
            _dbContext.Categoria.Add(categoria);
            return await _dbContext.SaveChangesAsync();
        }

        // Actualizar una Categoría existente
        public async Task<int> UpdateAsync(Categoria categoria)
        {
            var categoriaBD = await _dbContext.Categoria.FirstOrDefaultAsync(c => c.Id == categoria.Id);

            if (categoriaBD == null) return 0;

            categoriaBD.Nombre = categoria.Nombre;
            _dbContext.Update(categoriaBD);
            return await _dbContext.SaveChangesAsync();
        }

        // Eliminar una Categoría (verifica tareas asociadas)
        public async Task<int> DeleteAsync(Categoria categoria)
        {
            var categoriaBD = await _dbContext.Categoria.FirstOrDefaultAsync(c => c.Id == categoria.Id);

            if (categoriaBD == null) return 0;

            bool isAssociatedWithTarea = await _dbContext.Tarea.AnyAsync(t => t.IdCategoria == categoriaBD.Id);
            if (isAssociatedWithTarea)
                throw new Exception("No se puede eliminar la categoría porque está asociada con una tarea.");

            _dbContext.Categoria.Remove(categoriaBD);
            return await _dbContext.SaveChangesAsync();
        }

        // Obtener una Categoría por ID
        public async Task<Categoria> GetByIdAsync(Categoria categoria)
        {
            return await _dbContext.Categoria.FirstOrDefaultAsync(c => c.Id == categoria.Id);
        }

        // Obtener todas las Categorías
        public async Task<List<Categoria>> GetAllAsync()
        {
            return await _dbContext.Categoria.ToListAsync();
        }

        // Construir consulta dinámica (para búsquedas)
        internal static IQueryable<Categoria> QuerySelect(IQueryable<Categoria> query, Categoria category)
        {
            if (category.Id > 0)
                query = query.Where(c => c.Id == category.Id);

            if (!string.IsNullOrWhiteSpace(category.Nombre))
                query = query.Where(c => c.Nombre.Contains(category.Nombre));

            query = query.OrderByDescending(c => c.Id);

            if (category.Top_Aux > 0)
                query = query.Take(category.Top_Aux);

            return query;
        }

        // Buscar Categorías según filtros dinámicos
        public async Task<List<Categoria>> SearchAsync(Categoria category)
        {
            var select = _dbContext.Categoria.AsQueryable();
            select = QuerySelect(select, category);
            return await select.ToListAsync();
        }
    }
}

