using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestordeTaras.EN
{
    public class ComentarioTareaReaccion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdComentario { get; set; }

        [Required]
        public int IdUsuario { get; set; }

        [Required]
        public byte TipoReaccion { get; set; } // 1=Like, 2=Love, 3=Incógnito

        [Required]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [ForeignKey("IdComentario")]
        public ComentarioTarea Comentario { get; set; }

        [ForeignKey("IdUsuario")]
        public Usuario Usuario { get; set; }
    }



}
