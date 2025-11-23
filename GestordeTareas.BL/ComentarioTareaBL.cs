using GestordeTaras.EN;
using GestordeTareas.BL.Interfaces;
using GestordeTareas.DAL.Interfaces;
using Microsoft.Extensions.Logging;

namespace GestordeTareas.BL
{
    public class ComentarioTareaBL : IComentarioTareaBL
    {
        private readonly IComentarioTarea _comentDal;
        private readonly ILogger<ComentarioTareaBL> _logger;

        public ComentarioTareaBL(IComentarioTarea comentDal, ILogger<ComentarioTareaBL> logger)
        {
            _comentDal = comentDal;
            _logger = logger;
        }
        public async Task<ComentarioTarea> CrearComentarioAsync(ComentarioTarea coment)
        {
            // ---------las validaciones ---
            if (coment == null)
                throw new ArgumentNullException(nameof(coment));

            if (coment.IdUsuario <= 0)
                throw new ArgumentException("El usuario que crea el comentario no es válido.");

            if (coment.IdTarea <= 0)
                throw new ArgumentException("La tarea del comentario no es válida.");

            if (string.IsNullOrWhiteSpace(coment.Comentario))
                throw new ArgumentException("El texto del comentario no puede estar vacío.");

            coment.Comentario = coment.Comentario.Trim();

            // --- REGLAS DE NEGOCIO ---
            coment.FechaCreacion = DateTime.UtcNow;
            coment.FechaModificacion = null;
            coment.Estado = 1;

            // --- DELEGAR A DAL (esta ya existencia del padre, etc) ---
            var creado = await _comentDal.CrearComentarioAsync(coment);

            if (creado == null)
                throw new Exception("No se pudo crear el comentario.");

            return creado;
        }

        /// <summary>
        /// Elimina un comentario y todo su subárbol (hijos, nietos, ...).
        /// Regla de negocio: solo el autor del comentario puede eliminar.
        /// Nota: la eliminación se realiza actualmente llamando a la DAL por cada id;
        /// para atomicidad y performance, implementar una eliminación en bloque en la DAL.
        /// </summary>
        public async Task<bool> EliminarComentarioTotalAsync(int idComent, int idUsuarioActual)
        {
            // Validaciones básicas
            if (idComent <= 0)
                throw new ArgumentException("El ID del comentario no es válido.", nameof(idComent));

            if (idUsuarioActual <= 0)
                throw new ArgumentException("El usuario actual no es válido.", nameof(idUsuarioActual));

            // 1) Obtener el comentario para validar autor y obtener IdTarea
            var comentario = await _comentDal.ObtenerComentarioPorIdAsync(idComent);
            if (comentario == null)
            {
                _logger?.LogWarning("Intento de eliminar comentario inexistente: {Id}", idComent);
                return false;
            }

            // 2) Regla de negocio: sólo autor puede eliminar
            if (comentario.IdUsuario != idUsuarioActual)
            {
                _logger?.LogWarning("Usuario {User} intentó eliminar comentario {Id} sin permiso.", idUsuarioActual, idComent);
                throw new UnauthorizedAccessException("No tienes permiso para eliminar este comentario.");
            }

            // 3) Obtener TODO el hilo de la tarea (construcción de árbol) para localizar el subárbol
            var hilo = await _comentDal.ObtenerHiloComentariosAsync(comentario.IdTarea);
            if (hilo == null)
            {
                _logger?.LogWarning("No se pudo obtener el hilo para la tarea {TaskId} al eliminar comentario {Id}.", comentario.IdTarea, idComent);
                throw new Exception("No se pudo obtener el hilo de la tarea.");
            }

            // 4) Buscar el nodo objetivo dentro del árbol y recolectar todos los ids del subárbol
            var idsAEliminar = new List<int>();
            bool encontrado = false;

            // Función recursiva para recorrer y, al encontrar, recolectar ids del subárbol
            void BuscarYRecolectar(ComentarioTarea nodo)
            {
                if (nodo == null) return;
                if (nodo.Id == idComent)
                {
                    // encontramos la rais del subarbol: recolectar
                    RecolectarSubarbol(nodo);
                    encontrado = true;
                    return;
                }

                if (nodo.Respuestas != null)
                {
                    foreach (var hijo in nodo.Respuestas)
                    {
                        // Si ya fue encontrado loo que hago es romper opcionalmente (pero seguimos para completar)
                        if (encontrado) break;
                        BuscarYRecolectar(hijo);
                    }
                }
            }

            // funcion que recolecta ids del subárbol (DFS)
            void RecolectarSubarbol(ComentarioTarea nodo)
            {
                if (nodo == null) return;
                idsAEliminar.Add(nodo.Id);
                if (nodo.Respuestas != null)
                {
                    foreach (var hijo in nodo.Respuestas)
                        RecolectarSubarbol(hijo);
                }
            }

            // Ejecutar busqueda sobre todas las raíces
            foreach (var raiz in hilo)
            {
                if (encontrado) break;
                BuscarYRecolectar(raiz);
            }

            if (!encontrado || idsAEliminar.Count == 0)
            {
                _logger?.LogWarning("No se encontró el subárbol para el comentario {Id} en la tarea {TaskId}.", idComent, comentario.IdTarea);
                return false;
            }

            // 5) Ordenar para eliminar hijos antes que padres (opcional si la BD hace cascade, pero seguro)
            // se elimina por profundidad: hojas primero. Una forma simple es eliminar en orden inverso de recorrido (idsAEliminar ya es pre-order),
            // así invertimos la lista para intentar borrar hojas antes que padres.
            idsAEliminar.Reverse();

            // 6) Ejecutar eliminación. Aquí se llama al DAL por cada id
            foreach (var id in idsAEliminar)
            {
                var ok = await _comentDal.EliminarComentarioTotalAsync(id);
                if (!ok)
                {
                    _logger?.LogError("Fallo al eliminar comentario hijo {Id} mientras se eliminaba subárbol de {RootId}.", id, idComent);
                    throw new Exception($"Fallo al eliminar comentario con id {id}.");
                }
            }

            _logger?.LogInformation("Subárbol de comentario {Id} eliminado correctamente (total {Count} items).", idComent, idsAEliminar.Count);
            return true;
        }

        public async Task<ComentarioTarea> ObtenerComentarioPorIdAsync(int idComent)
        {
            // --- VALIDACIÓN ---
            if (idComent <= 0)
                throw new ArgumentException("El ID del comentario no es válido.", nameof(idComent));

            // --- LOG opcional: intención ---
            _logger?.LogInformation("BL: Solicitando comentario con ID {Id}.", idComent);

            // --- CONSULTA A DAL ---
            var comentario = await _comentDal.ObtenerComentarioPorIdAsync(idComent);

            // --- MANEJO DE RESULTADO ---
            if (comentario == null)
            {
                _logger?.LogWarning("BL: No existe comentario con ID {Id}.", idComent);
                return null; // BL no debe forzar excepción por 'no encontrado' salvo regla explícita
            }

            return comentario;
        }

        public async Task<IEnumerable<ComentarioTarea>> ObtenerComentariosPorTareaAsync(int idTask)
        {
            // --- VALIDACIÓN ---
            if (idTask <= 0)
                throw new ArgumentException("El ID de la tarea no es válido.", nameof(idTask));

            // --- LOG DE INTENCIÓN ---
            _logger?.LogInformation("BL: Solicitando comentarios raíz de la tarea {Id}.", idTask);

            // --- CONSULTA A DAL ---
            var comentarios = await _comentDal.ObtenerComentariosPorTareaAsync(idTask);

            // --- MANEJO DEL RESULTADO ---
            if (comentarios == null || !comentarios.Any())
            {
                _logger?.LogWarning("BL: La tarea {Id} no tiene comentarios raíz.", idTask);
                return Enumerable.Empty<ComentarioTarea>();
            }

            return comentarios;
        }

        public async Task<IEnumerable<ComentarioTarea>> ObtenerHiloComentariosAsync(int idTask)
        {
            // --- VALIDACIÓN ---
            if (idTask <= 0)
                throw new ArgumentException("El ID de la tarea no es válido.", nameof(idTask));

            _logger?.LogInformation("BL: Solicitando hilo completo de comentarios para la tarea {Id}.", idTask);

            // --- CONSULTA A DAL ---
            var hilo = await _comentDal.ObtenerHiloComentariosAsync(idTask);

            // --- MANEJO DEL RESULTADO ---
            if (hilo == null || !hilo.Any())
            {
                _logger?.LogWarning("BL: La tarea {Id} no tiene comentarios asociados.", idTask);
                return Enumerable.Empty<ComentarioTarea>();
            }

            return hilo;
        }
        public async Task<int> ContarComentariosAsync(int idTask)
        {
            // --- VALIDACIÓN ---
            if (idTask <= 0)
                throw new ArgumentException("El ID de la tarea no es válido.", nameof(idTask));

            _logger?.LogInformation("BL: Contando comentarios para la tarea {Id}.", idTask);

            // --- LLAMADA A DAL ---
            var total = await _comentDal.ContarComentariosAsync(idTask);

            // --- RESULTADO ---
            return total;
        }
        public async Task<bool> EditarComentarioAsync(ComentarioTarea coment, int idUsuarioActual)
        {
            if (coment == null)
                throw new ArgumentNullException(nameof(coment), "El objeto comentario es nulo.");

            if (coment.Id <= 0)
                throw new ArgumentException("El ID del comentario no es válido.");

            if (string.IsNullOrWhiteSpace(coment.Comentario))
                throw new ArgumentException("El comentario no puede estar vacío.");

            if (idUsuarioActual <= 0)
                throw new ArgumentException("El usuario actual no es válido.");

            // Obtener desde DAL el comentario real para validar dueño
            var comentarioBD = await _comentDal.ObtenerComentarioPorIdAsync(coment.Id);

            if (comentarioBD == null)
                throw new Exception("El comentario no existe.");

            // Regla de negocio: solo el autor puede editar
            if (comentarioBD.IdUsuario != idUsuarioActual)
                throw new UnauthorizedAccessException("No tienes permiso para editar este comentario.");

            // Preparar el modelo actualizado (solo campos permitidos)
            comentarioBD.Comentario = coment.Comentario.Trim();
            comentarioBD.FechaModificacion = DateTime.UtcNow;

            // Llamar a DAL
            var actualizado = await _comentDal.EditarComentarioAsync(comentarioBD);

            if (!actualizado)
                throw new Exception("No se pudo editar el comentario.");

            return true;
        }

    }
}
