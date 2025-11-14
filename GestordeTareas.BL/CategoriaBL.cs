using GestordeTaras.EN;
using GestordeTareas.DAL.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestordeTareas.BL
{
    public class CategoriaBL
    {
        private readonly ICategoria _categoriaDAL;

        public CategoriaBL(ICategoria categoriaDAL)
        {
            _categoriaDAL = categoriaDAL;
        }

        public async Task<Categoria> CreateAsync(Categoria categoria)
        {
            return await _categoriaDAL.CreateCategoriaAsync(categoria);
        }

        public async Task<Categoria> UpdateAsync(Categoria categoria)
        {
            return await _categoriaDAL.UpdateCategoriaAsync(categoria);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _categoriaDAL.DeleteCategoriaAsync(id);
        }

        public async Task<Categoria> GetByIdAsync(int id)
        {
            return await _categoriaDAL.GetCategoriaByIdAsync(id);
        }

        public async Task<IEnumerable<Categoria>> GetAllAsync()
        {
            return await _categoriaDAL.GetAllCategoriasAsync();
        }
    }
}
