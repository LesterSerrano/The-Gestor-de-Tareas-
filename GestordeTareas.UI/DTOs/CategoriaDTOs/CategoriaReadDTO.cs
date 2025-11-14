using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestordeTareas.UI.DTOs.CategoriaDTOs
{
    public class CategoriaReadDTO
    {
          public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        // Opcional: esto es por si se nos viene en gana devolver cuantas tareas tiene una categoria
        public int CantidadTareas { get; set; }
    }
}