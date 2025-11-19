using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestordeTaras.EN
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Foto")]
        public string? FotoPerfil { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [MaxLength(50, ErrorMessage = "Maximo 50 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(50, ErrorMessage = "Maximo 50 caracteres")]
        public string? Apellido { get; set; }  

        [Required(ErrorMessage = "Campo obligatorio")]
        [MaxLength(100, ErrorMessage = "Maximo 100 caracteres")]
        [Display(Name = "Correo Electrónico")]
        public string NombreUsuario { get; set; } = string.Empty;

        public string? Pass { get; set; }

        [NotMapped]
        public string? ConfirmarPass { get; set; }

        [MaxLength(20, ErrorMessage = "Maximo 20 caracteres")]
        public string? Telefono { get; set; } 

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha de nacimiento")]
        public DateTime? FechaNacimiento { get; set; } = DateTime.Now;  

        [Required(ErrorMessage = "El estado es requerido")]
        [Display(Name = "Estado")]
        public byte Status { get; set; }

        [Display(Name = "Fecha de registro")]
        public DateTime FechaRegistro { get; set; }

        [ForeignKey("Cargo")]
        [Required(ErrorMessage = "Campo obligatorio")]
        [Display(Name = "Cargo")]
        public int IdCargo { get; set; }

        [NotMapped]
        public int Top_Aux { get; set; }

        [NotMapped]
        public string? ConfirmPassword_Aux { get; set; }

        public Cargo Cargo { get; set; }
        public virtual ICollection<ProyectoUsuario> ProyectoUsuario { get; set; }
        public ICollection<PasswordResetCode> PasswordResetCode { get; set; }
        public ICollection<Comment> Comment { get; set; }
    }
    public enum User_Status
    {
        ACTIVO = 1, INACTIVO = 2
    }

}

