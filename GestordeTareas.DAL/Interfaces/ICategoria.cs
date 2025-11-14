using System.Collections.Generic;
using GestordeTaras.EN;

namespace GestordeTareas.DAL.Interfaces
{
    public interface ICategoria
    {
        Task<Categoria> GetCategoriaByIdAsync(int id);
        Task<IEnumerable<Categoria>> GetAllCategoriasAsync();
        Task<Categoria> CreateCategoriaAsync(Categoria categoria);
        Task<Categoria> UpdateCategoriaAsync(Categoria categoria);
        Task<bool> DeleteCategoriaAsync(int id);
    // metodo para paginacion
    // Task<IEnumerable<Categoria>> GetCategoriasByPageAsync(int pageNumber, int pageSize);
    }
}