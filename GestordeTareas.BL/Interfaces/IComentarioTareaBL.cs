using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestordeTaras.EN;

namespace GestordeTareas.BL.Interfaces
{
    public interface IComentarioTareaBL
    {
        Task<ComentarioTarea> CrearComentarioAsync(ComentarioTarea coment);
        Task<bool> EditarComentarioAsync(ComentarioTarea coment, int idUsuarioActual);
        Task<bool> EliminarComentarioTotalAsync(int idComent, int idUsuarioActual);
        Task<ComentarioTarea> ObtenerComentarioPorIdAsync(int idComent);
        Task<IEnumerable<ComentarioTarea>> ObtenerComentariosPorTareaAsync(int idTask);
        Task<IEnumerable<ComentarioTarea>> ObtenerHiloComentariosAsync(int idTask);
        Task<int> ContarComentariosAsync(int idTask);
    }

}