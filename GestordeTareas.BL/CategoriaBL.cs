using GestordeTaras.EN;
<<<<<<< HEAD
using GestordeTareas.DAL.Interfaces;
=======
using GestordeTareas.DAL;
using GestordeTareas.DAL.Interfaces;
using System;
>>>>>>> 6f3904e1ef769397f15b3f638d17e031b23152fb
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestordeTareas.BL
{
    public class CategoriaBL
    {
<<<<<<< HEAD
        private readonly ICategoria _categoriaDAL;

        public CategoriaBL(ICategoria categoriaDAL)
        {
            _categoriaDAL = categoriaDAL;
        }

        public async Task<Categoria> CreateAsync(Categoria categoria)
        {
            return await _categoriaDAL.CreateCategoriaAsync(categoria);
=======
        private readonly ICategoriaDAL _CategoriaDAL;

        public CategoriaBL(ICategoriaDAL categoriaDAL)
        {
            _CategoriaDAL = categoriaDAL;
        }
        public async Task<int> CreateAsync(Categoria categoria)
        {
            return await _CategoriaDAL.CreateAsync(categoria);
        }
        public async Task<int> UpdateAsync(Categoria categoria)
        {
            return await _CategoriaDAL.UpdateAsync(categoria);
        }
        public async Task<int> DeleteAsync(Categoria categoria)
        {
            return await _CategoriaDAL.DeleteAsync(categoria);
        }

        public async Task<Categoria> GetByIdAsync(Categoria categoria)
        {
            return await _CategoriaDAL.GetByIdAsync(categoria);
>>>>>>> 6f3904e1ef769397f15b3f638d17e031b23152fb
        }

        public async Task<Categoria> UpdateAsync(Categoria categoria)
        {
<<<<<<< HEAD
            return await _categoriaDAL.UpdateCategoriaAsync(categoria);
=======
            return await _CategoriaDAL.GetAllAsync();
>>>>>>> 6f3904e1ef769397f15b3f638d17e031b23152fb
        }

        public async Task<bool> DeleteAsync(int id)
        {
<<<<<<< HEAD
            return await _categoriaDAL.DeleteCategoriaAsync(id);
        }

        public async Task<Categoria> GetByIdAsync(int id)
        {
            return await _categoriaDAL.GetCategoriaByIdAsync(id);
        }

        public async Task<IEnumerable<Categoria>> GetAllAsync()
        {
            return await _categoriaDAL.GetAllCategoriasAsync();
=======
            return await _CategoriaDAL.SearchAsync(category);
>>>>>>> 6f3904e1ef769397f15b3f638d17e031b23152fb
        }
    }
}
