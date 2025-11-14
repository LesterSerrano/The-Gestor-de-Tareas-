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
    public class CategoriaDAL : ICategoria
    {
        /// <summary>
        ///  Task<Categoria> GetCategoriaByIdAsync(Guid id);
        // Task<IEnumerable<Categoria>> GetAllCategoriasAsync();
        // Task<Categoria> CreateCategoriaAsync(Categoria Categoria);
        // Task<Categoria> UpdateCategoriaAsync(Categoria Categoria);
        // Task<bool> DeleteCategoriaAsync(Guid id);
        /// </summary>
        private readonly ContextoBD _dbContext;
        private readonly ILogger<CategoriaDAL> _logger;
        public CategoriaDAL(ContextoBD dbContext, ILogger<CategoriaDAL> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        //traer todas las categorias
        public async Task<Categoria> GetCategoriaByIdAsync(int id)
        {
            var categoria = await _dbContext.Categoria
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);
            if (categoria == null)
            {
                _logger.LogWarning($"Categoria con ID {id} no encontrado.");
                return new Categoria
                {
                    Id = 0,
                    Nombre = string.Empty
                };
            }
            return categoria;
        }
        public async Task<IEnumerable<Categoria>> GetAllCategoriasAsync()
        {
            var categorias = await _dbContext.Categoria
                .AsNoTracking()
                .ToListAsync();
            if (categorias.Count == 0)
            {
                _logger.LogWarning("No se encontraron categorias.");
            }
            return categorias;
        }
        public async Task<Categoria> CreateCategoriaAsync(Categoria categoria)
        {
            // Validar si ya existe una categoría con ese nombre
            var existeCategoria = await _dbContext.Categoria
                .AsNoTracking()
                .AnyAsync(c => c.Nombre.ToLower() == categoria.Nombre.ToLower());

            if (existeCategoria)
            {
                _logger.LogWarning($"La categoría con nombre '{categoria.Nombre}' ya existe.");

                // Retornar un objeto de categoria para decir: "¡weeeeeeeeeeeeeee esto ya existeeeee no seas weoooon!"
                return new Categoria
                {
                    Id = 0,
                    Nombre = "EXISTE"
                };
            }

            // Agregar y guardar
            _dbContext.Categoria.Add(categoria);
            await _dbContext.SaveChangesAsync();

            // Retornar la categoría creada con su Id generado
            return categoria;
        }


        public async Task<Categoria> UpdateCategoriaAsync(Categoria categoria)
        {
            var categoriaBD = await GetCategoriaByIdAsync(categoria.Id); //aqui uso el metodo de traer por Id

            // Verificar si el nuevo nombre ya existe en otra categoria asi como el de creat
            var nombreEnUso = await _dbContext.Categoria
                .AsNoTracking()
                .AnyAsync(c => c.Nombre.ToLower() == categoria.Nombre.ToLower() && c.Id != categoria.Id);

            if (nombreEnUso)
            {
                _logger.LogWarning($"No se puede actualizar. El nombre '{categoria.Nombre}' ya está en uso por otra categoría.");

                return new Categoria
                {
                    Id = 0,
                    Nombre = "NOMBRE_DUPLICADO"
                };
            }
            // Actualizacion en cuestion
            categoriaBD.Nombre = categoria.Nombre;

            _dbContext.Update(categoriaBD);
            await _dbContext.SaveChangesAsync();

            return categoriaBD;
        }

        public async Task<bool> DeleteCategoriaAsync(int id)
        {
            var categoriaBD = await GetCategoriaByIdAsync(id);

            if (categoriaBD.Id == 0)
            {
                _logger.LogWarning($"No se puede eliminar. La categoría con ID {id} no existe.");
                return false;
            }

            // Verificar si hay tareas asociadas a esta categoría
            bool tieneTareasAsociadas = await _dbContext.Tarea
                .AsNoTracking()
                .AnyAsync(t => t.IdCategoria == categoriaBD.Id);

            if (tieneTareasAsociadas)
            {
                _logger.LogWarning($"No se puede eliminar la categoría con ID {id} porque tiene tareas asociadas.");
                return false;
            }

            _dbContext.Categoria.Remove(categoriaBD);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }

}




















