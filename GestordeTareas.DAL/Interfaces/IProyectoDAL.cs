using GestordeTaras.EN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestordeTareas.DAL.Interfaces
{
    public interface IProyectoDAL
    {
        Task<int> CreateAsync(Proyecto proyecto);
        Task<int> UpdateAsync(Proyecto proyecto);
        Task<int> DeleteAsync(Proyecto proyecto);
        Task<Proyecto> GetByIdAsync(Proyecto proyecto);
        Task<List<Proyecto>> GetAllAsync();
        Task<bool> ExisteCodigoAccesoAsync(string codigoAcceso);
        Task<List<Proyecto>> BuscarPorTituloOAdministradorAsync(string query);
    }

}
