using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestordeTaras.EN
{
    public class ComentarioTarea
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdTarea { get; set; }

        [Required]
        public int IdUsuario { get; set; }

        [Required]
        [MaxLength(500)]
        public string Comentario { get; set; }

        [Required]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public int? IdComentarioPadre { get; set; }

        [Required]
        public byte Estado { get; set; } = 1;

        // 🔗 Relaciones

        [ForeignKey("IdTarea")]
        public Tarea Tarea { get; set; }

        [ForeignKey("IdUsuario")]
        public Usuario Usuario { get; set; }

        [ForeignKey("IdComentarioPadre")]
        public ComentarioTarea ComentarioPadre { get; set; }

        public ICollection<ComentarioTarea> Respuestas { get; set; } = new List<ComentarioTarea>();

        public ICollection<ComentarioTareaReaccion> Reacciones { get; set; } = new List<ComentarioTareaReaccion>();
    }
}
