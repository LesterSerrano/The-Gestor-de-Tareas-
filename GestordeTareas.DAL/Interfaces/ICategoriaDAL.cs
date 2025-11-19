using GestordeTaras.EN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestordeTareas.DAL.Interfaces
{
    public interface ICategoriaDAL
    {
        Task<int> CreateAsync(Categoria categoria);
        Task<int> UpdateAsync(Categoria categoria);
        Task<int> DeleteAsync(Categoria categoria);
        Task<Categoria> GetByIdAsync(Categoria categoria);
        Task<List<Categoria>> GetAllAsync();
        Task<List<Categoria>> SearchAsync(Categoria category);

    }
}
