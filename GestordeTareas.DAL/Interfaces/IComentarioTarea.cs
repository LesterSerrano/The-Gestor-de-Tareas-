using System.Collections.Generic;
using System.Threading.Tasks;
using GestordeTaras.EN;

namespace GestordeTareas.DAL.Interfaces
{
    public interface IComentarioTarea
    {
        // Crear comentario
        Task<int> CrearComentarioAsync(ComentarioTarea comentario);

        // Editar el texto del comentario
        Task<bool> EditarComentarioAsync(ComentarioTarea comentario);

        // Eliminar completamente (DELETE físico en DB)
        Task<bool> EliminarComentarioTotalAsync(int idComentario);

        // Obtener un comentario específico por ID
        Task<ComentarioTarea> ObtenerPorIdAsync(int idComentario);

        // Obtener todos los comentarios de una tarea
        Task<IEnumerable<ComentarioTarea>> ObtenerComentariosPorTareaAsync(int idTarea);

        // Obtener comentarios organizados como hilo (padres + respuestas y aqui en la vista vere las reaacoines)
        Task<IEnumerable<ComentarioTarea>> ObtenerHiloComentariosAsync(int idTarea);

        // Contar comentarios asociados a una tarea
        Task<int> ContarComentariosAsync(int idTarea);
    }
}
