using GestordeTaras.EN;
using GestordeTareas.DAL.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestordeTareas.BL
{
    public class ProyectoBL
    {
        private readonly IProyectoDAL _proyectoDAL;

        public ProyectoBL(IProyectoDAL proyectoDAL)
        {
            _proyectoDAL = proyectoDAL;
        }

        public async Task<int> CreateAsync(Proyecto proyecto)
        {
            return await _proyectoDAL.CreateAsync(proyecto);
        }

        public async Task<int> UpdateAsync(Proyecto proyecto)
        {
            return await _proyectoDAL.UpdateAsync(proyecto);
        }

        public async Task<int> DeleteAsync(Proyecto proyecto)
        {
            return await _proyectoDAL.DeleteAsync(proyecto);
        }

        public async Task<Proyecto> GetByIdAsync(Proyecto proyecto)
        {
            return await _proyectoDAL.GetByIdAsync(proyecto);
        }

        public async Task<List<Proyecto>> GetAllAsync()
        {
            return await _proyectoDAL.GetAllAsync();
        }       

        public async Task<bool> ExisteCodigoAccesoAsync(string codigoAcceso)
        {
            return await _proyectoDAL.ExisteCodigoAccesoAsync(codigoAcceso);
        }

        public async Task<List<Proyecto>> BuscarPorTituloOAdministradorAsync(string query)
        {
            return await _proyectoDAL.BuscarPorTituloOAdministradorAsync(query);
        }

        public string GenerarCodigoAcceso()
        {
            const string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var codigo = new char[8];

            for (int i = 0; i < codigo.Length; i++)
                codigo[i] = caracteres[random.Next(caracteres.Length)];

            return new string(codigo);
        }
    }
}
