using System.Collections.Generic;
using System.Threading.Tasks;
using GestordeTaras.EN;

namespace GestordeTareas.DAL.Interfaces
{
    public interface IComentarioTarea
    {
        // Crear comentario
        Task<ComentarioTarea> CrearComentarioAsync(ComentarioTarea _coment);

        // Editar el texto del comentario
        Task<bool> EditarComentarioAsync(ComentarioTarea _coment);

        // Eliminar completamente (DELETE físico en DB)
        Task<bool> EliminarComentarioTotalAsync(int idComent);


        // Obtener un comentario específico por ID
        Task<ComentarioTarea> ObtenerComentarioPorIdAsync(int idComent);

        // Obtener todos los comentarios de una tarea
        Task<IEnumerable<ComentarioTarea>> ObtenerComentariosPorTareaAsync(int idTask);

        // Obtener comentarios organizados como hilo (padres + respuestas y aqui en la vista vere las reaacoines)
        Task<IEnumerable<ComentarioTarea>> ObtenerHiloComentariosAsync(int idTask);

        // Contar comentarios asociados a una tarea
        Task<int> ContarComentariosAsync(int idTask);
    }
}
