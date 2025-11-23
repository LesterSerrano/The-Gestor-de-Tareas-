using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestordeTaras.EN;
using Microsoft.EntityFrameworkCore;

namespace GestordeTareas.DAL.Utils
{// Helper de navegación para ComentarioTarea.
 // Este metoodo de extencion centraliza todos los Includes necesarios para
 // cargar un comentario completo con su usuario, reacciones y respuestas.
    
    public static class ComentarioIncludes //vieeeeejooooooooooooooo Static = no instancias la clase, pero si se devuelven valores
    {
        public static IQueryable<ComentarioTarea> ConTodosLosIncludes(this IQueryable<ComentarioTarea> query)
        {
            return query
                .Include(c => c.Usuario)            // Usuario que hizo el comentario
                .Include(c => c.Reacciones)         // Reacciones al comentario
                .Include(c => c.Respuestas)         // Respuestas del comentario
                    .ThenInclude(r => r.Usuario)    // Usuario que responde
                .Include(c => c.Respuestas)
                    .ThenInclude(r => r.Reacciones); // Reacciones a las respuestas
        }
    }

}