using GestordeTaras.EN;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestordeTareas.UI.DTOs
{
    public class UsuarioCreateDTO
    {
        [Display(Name = "Foto")]
        [Required(ErrorMessage = "Campo obligatorio")]
        public string FotoPerfil { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo obligatorio")]
        [MaxLength(50, ErrorMessage = "Maximo 50 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo obligatorio")]
        [MaxLength(50, ErrorMessage = "Maximo 50 caracteres")]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo obligatorio")]
        [MaxLength(100, ErrorMessage = "Maximo 100 caracteres")]
        [Display(Name = "Correo Electrónico")]
        public string NombreUsuario { get; set; } = string.Empty; // Correo Electrónico

        [Required(ErrorMessage = "Campo obligatorio")]
        [Display(Name = "Contraseña")]
        [StringLength(32, ErrorMessage = "La contraseña debe tener entre 6 y 32 caracteres", MinimumLength = 6)] // Validaciones de tu modelo
        [DataType(DataType.Password)]
        public string Pass { get; set; } = string.Empty;

        [NotMapped]
        [Required(ErrorMessage = "La confirmación de contraseña es requerida")]
        [DataType(DataType.Password)]
        [StringLength(32, ErrorMessage = "La contraseña debe tener entre 6 y 32 caracteres", MinimumLength = 6)]
        [Compare("Pass", ErrorMessage = "Las contraseñas no coinciden")]
        [Display(Name = "Confirmar la contraseña")]
        public string ConfirmarPass { get; set; } = string.Empty;

        [MaxLength(20, ErrorMessage = "Maximo 20 caracteres")]
        [Required(ErrorMessage = "Campo obligatorio")]
        public string Telefono { get; set; } = string.Empty;

        [Display(Name = "Fecha de nacimiento")]
        [Required(ErrorMessage = "Campo obligatorio")]
        [DataType(DataType.Date)] 
        public DateTime FechaNacimiento { get; set; } = DateTime.Now;

        [ForeignKey("Cargo")]
        [Required(ErrorMessage = "Campo obligatorio")]
        [Display(Name = "Cargo")]
        public int IdCargo { get; set; }
    }
}