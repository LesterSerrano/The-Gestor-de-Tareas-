using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestordeTaras.EN;
using GestordeTareas.DAL.Interfaces;
using GestordeTareas.DAL.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GestordeTareas.DAL
{
    public class ComentarioTareaDAL : IComentarioTarea
    {
        private readonly ContextoBD _contextoBD;
        private readonly ILogger<ComentarioTareaDAL> _logger;
        public ComentarioTareaDAL(ILogger<ComentarioTareaDAL> logger, ContextoBD contextoBD)
        {
            _logger = logger;
            _contextoBD = contextoBD;
        }
        public async Task<ComentarioTarea> CrearComentarioAsync(ComentarioTarea _coment)
        {
            try
            {
                // ---------- aqiui ocupo lo de los utilis  pero para vaidaciones generales netamente de la clase ----------
                ValidateComentarioTarea.Validar(_coment, _logger);

                // ---------- FECHA DE CREACIÓN ----------
                _coment.FechaCreacion = DateTime.UtcNow;

                // ---------- VALIDAR QUE EL PADRE EXISTA (si trae uno valido o el se vuelve papaa) ----------
                if (_coment.IdComentarioPadre.HasValue)
                {
                    bool existePadre = await _contextoBD.ComentarioTarea
                        .AnyAsync(c => c.Id == _coment.IdComentarioPadre);

                    if (!existePadre)
                    {
                        _logger.LogWarning("El comentario padre con ID {Id} no existe.", _coment.IdComentarioPadre);
                        throw new ArgumentException("El comentario padre no existe.");
                    }
                }

                // ---------- INSERTAR EN BASE DE DATOS ----------
                await _contextoBD.ComentarioTarea.AddAsync(_coment);
                await _contextoBD.SaveChangesAsync();

                _logger.LogInformation("Comentario creado correctamente con ID {Id}.", _coment.Id);

                return _coment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el comentario.");
                throw;
            }
        }
        /// <summary>
        /// Elimina físicamente un comentario de la base de datos,
        /// usando solo su ID. Retorna true si la operación fue exitosa.
        /// </summary>
        public async Task<bool> EliminarComentarioTotalAsync(int idComent)
        {
            try
            {
                if (idComent <= 0)
                    throw new ArgumentException("El ID proporcionado no es válido.", nameof(idComent));

                var comentarioBD = await _contextoBD.ComentarioTarea
                    .FirstOrDefaultAsync(c => c.Id == idComent);

                if (comentarioBD == null)
                    return false;

                _contextoBD.ComentarioTarea.Remove(comentarioBD);

                var cambios = await _contextoBD.SaveChangesAsync();
                return cambios > 0;
            }
            catch
            {
                throw;
            }
        }

        public async Task<ComentarioTarea> ObtenerComentarioPorIdAsync(int idComent)
        {
            if (idComent <= 0)
            {
                _logger.LogWarning("ID de comentario inválido: {Id}", idComent);
                throw new ArgumentException("El ID del comentario es inválido.");
            }

            try
            {
                var comentario = await _contextoBD.ComentarioTarea
                    .AsNoTracking()
                    .Where(c => c.Id == idComent)
                    .ConTodosLosIncludes()
                    .FirstOrDefaultAsync();

                if (comentario == null)
                {
                    _logger.LogWarning("No se encontró un comentario con ID {Id}.", idComent);
                    return null;
                }

                return comentario;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el comentario con ID {Id}.", idComent);
                throw;
            }
        }


        public async Task<IEnumerable<ComentarioTarea>> ObtenerComentariosPorTareaAsync(int idTask)
        {
            try
            {
                var comentarios = await _contextoBD.ComentarioTarea
                .AsNoTracking()
                .Where(c => c.IdTarea == idTask && c.IdComentarioPadre == null)
                .ConTodosLosIncludes()
                .OrderByDescending(c => c.FechaCreacion)
                .ToListAsync();

                return comentarios;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los comentarios para la tarea con ID {Id}.", idTask);
                throw;
            }
        }

        /// <summary>
        /// Obtiene el hilo completo de comentarios de una tarea aplicando el
        /// patrón Specification para el filtrado inicial y el patrón Composite
        /// para estructurar las respuestas en un árbol jerárquico. El método
        /// retorna una colección optimizada de comentarios raíz lista para
        /// consumo en la capa de presentacion
        /// 
        /// -------las cosas estas estan e el utils/ComentariosDeTareaSpec.cs--------
        /// </summary>
        public async Task<IEnumerable<ComentarioTarea>> ObtenerHiloComentariosAsync(int idTask)
        {
            try
            {
                // Uso de la especificacion que cree en utils
                var spec = new ComentariosDeTareaSpec(idTask);
                var todos = await spec.Apply(_contextoBD.ComentarioTarea)
                    .AsNoTracking()
                    .OrderBy(c => c.FechaCreacion)
                    .ToListAsync();

                if (todos.Count == 0)
                    return Enumerable.Empty<ComentarioTarea>();

                // Armamos el arbol Composite
                var dict = todos.ToDictionary(c => c.Id);
                var raiz = new List<ComentarioTarea>();

                foreach (var c in todos)
                {
                    if (c.IdComentarioPadre == null)
                        raiz.Add(c);

                    else if (dict.TryGetValue(c.IdComentarioPadre.Value, out var padre))
                    {
                        padre.Respuestas ??= new List<ComentarioTarea>();
                        padre.Respuestas.Add(c);
                    }
                }

                return raiz;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el hilo de comentarios para la tarea {Id}.", idTask);
                throw;
            }
        }
        /// <summary>
        /// Devuelve la cantidad total de comentarios relacionados a una tarea.
        /// Utiliza una consulta optimizada con seguimiento deshabilitado para 
        /// mejorar el rendimiento.
        /// </summary>

        public async Task<int> ContarComentariosAsync(int idTask)
        {
            try
            {
                return await _contextoBD.ComentarioTarea
                    .AsNoTracking()
                    .CountAsync(c => c.IdTarea == idTask);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al contar los comentarios de la tarea {Id}.", idTask);
                throw;
            }
        }
        /// <summary>
        /// Actualiza el texto de un comentario existente.
        /// Usa ObtenerComentarioPorIdAsync para evitar duplicar lógica de acceso,
        /// valida la entrada y registra la fecha de modificación.
        /// </summary>
        public async Task<bool> EditarComentarioAsync(ComentarioTarea _coment)
        {
            try
            {
                if (_coment == null)
                    throw new ArgumentNullException(nameof(_coment), "El objeto comentario es nulo.");

                if (_coment.Id <= 0)
                    throw new ArgumentException("El ID del comentario es inválido.", nameof(_coment.Id));

                if (string.IsNullOrWhiteSpace(_coment.Comentario))
                    throw new ArgumentException("El texto del comentario no puede estar vacío.");

                // Obtener desde la capa existente (DRY)
                var comentarioBD = await ObtenerComentarioPorIdAsync(_coment.Id);

                if (comentarioBD == null)
                {
                    _logger.LogWarning("No se encontró un comentario con ID {Id} para editar.", _coment.Id);
                    return false;
                }

                // Actualiza solo lo permitido
                comentarioBD.Comentario = _coment.Comentario.Trim();
                comentarioBD.FechaModificacion = DateTime.UtcNow;

                _contextoBD.ComentarioTarea.Update(comentarioBD);

                var cambios = await _contextoBD.SaveChangesAsync();

                _logger.LogInformation("Comentario {Id} editado correctamente.", _coment.Id);
                return cambios > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al editar el comentario con ID {Id}.", _coment?.Id);
                throw;
            }
        }

    }
}