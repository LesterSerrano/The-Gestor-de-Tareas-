using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestordeTaras.EN;
using Microsoft.EntityFrameworkCore;

namespace GestordeTareas.DAL.Utils
{
    /// <summary>
    /// Encapsula todos los Includes del comentario COMPLETO.

    // Encapsula el filtro por tarea.

    // Deja el repositorio totalmente limpio bueno el service
    /// </summary>
    public class ComentariosDeTareaSpec
    {
        public int IdTarea { get; }

        public ComentariosDeTareaSpec(int idTarea)
        {
            IdTarea = idTarea;
        }

        public IQueryable<ComentarioTarea> Apply(IQueryable<ComentarioTarea> query)
        {
            return query
                .Where(c => c.IdTarea == IdTarea)
                .Include(c => c.Usuario)
                .Include(c => c.Reacciones)
                .Include(c => c.Respuestas)
                    .ThenInclude(r => r.Usuario)
                .Include(c => c.Respuestas)
                    .ThenInclude(r => r.Reacciones);
        }
    }
}