using System.Collections.Generic;
using System.Threading.Tasks;
using GestordeTaras.EN;

namespace GestordeTareas.DAL.Interfaces
{
    public interface IComentarioTareaReaccion
    {
        // Crear o registrar una reaacion de me encachimba al comperantio
        Task<int> CrearReaccionAsync(ComentarioTareaReaccion reaccion);

        // Actualizar el tipo de reaacin (Like → Love, etc.)
        Task<bool> ActualizarReaccionAsync(ComentarioTareaReaccion reaccion);

        // Eliminar 
        Task<bool> EliminarReaccionAsync(int idReaccion);

        // Obtener reaacion de un usuario sobre un comentario especific
        Task<ComentarioTareaReaccion> ObtenerReaccionUsuarioAsync(int idComentario, int idUsuario);

        // Obtener todas las reacciones de un comentario
        Task<IEnumerable<ComentarioTareaReaccion>> ObtenerReaccionesPorComentarioAsync(int idComentario);

        // Contar reacciones de un comentario
        Task<int> ContarReaccionesAsync(int idComentario);

        // Resumen de reacciones agrupadas por tipo (like, love, dudaaas)
        Task<Dictionary<byte, int>> ObtenerResumenReaccionesAsync(int idComentario);
    }
}
