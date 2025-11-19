using GestordeTaras.EN;
using GestordeTareas.BL;
using Microsoft.AspNetCore.Mvc;

namespace GestordeTareas.UI.Controllers
{
    public class ProyectoUsuarioController : Controller
    {
        private readonly ProyectoUsuarioBL _proyectoUsuarioBL;
        private readonly UsuarioBL _usuarioBL;
        private readonly ProyectoBL _proyectoBL;

        public ProyectoUsuarioController(ProyectoUsuarioBL proyectoUsuarioBL, UsuarioBL usuarioBL, ProyectoBL proyectoBL)
        {
            _proyectoUsuarioBL = proyectoUsuarioBL;
            _usuarioBL = usuarioBL;
            _proyectoBL = proyectoBL;
        }

        [HttpPost]
        public async Task<IActionResult> UnirUsuarioAProyecto(int idProyecto, string codigoAcceso)
        {
            var actualUser = (await _usuarioBL.SearchAsync(new Usuario { NombreUsuario = User.Identity.Name, Top_Aux = 1 }))
                                .FirstOrDefault();

            if (actualUser == null) return NotFound();

            var proyecto = await _proyectoBL.GetByIdAsync(new Proyecto { Id = idProyecto });
            if (proyecto == null) return NotFound();

            if (proyecto.CodigoAcceso != codigoAcceso)
            {
                TempData["ErrorMessage"] = "El código de acceso es incorrecto.";
                return RedirectToAction("Details", "Proyecto", new { id = idProyecto });
            }

            var usuariosUnidos = await _proyectoUsuarioBL.ObtenerUsuariosUnidosAsync(idProyecto);
            if (usuariosUnidos.Any(u => u.Id == actualUser.Id))
            {
                TempData["ErrorMessage"] = "Ya perteneces a este proyecto.";
                return RedirectToAction("Details", "Proyecto", new { id = idProyecto });
            }

            var result = await _proyectoUsuarioBL.UnirUsuarioAProyectoAsync(idProyecto, actualUser.Id);
            TempData[result > 0 ? "SuccessMessage" : "ErrorMessage"] =
                result > 0 ? "Te has unido al proyecto exitosamente." : "No se pudo unir al proyecto.";

            return RedirectToAction("Details", "Proyecto", new { id = idProyecto });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalirDeProyecto(int idProyecto)
        {
            var actualUser = (await _usuarioBL.SearchAsync(new Usuario { NombreUsuario = User.Identity.Name, Top_Aux = 1 }))
                                .FirstOrDefault();
            if (actualUser == null) return NotFound();

            var result = await _proyectoUsuarioBL.EliminarUsuarioDeProyectoAsync(idProyecto, actualUser.Id);
            TempData[result > 0 ? "SuccessMessage" : "ErrorMessage"] =
                result > 0 ? "Has salido del proyecto exitosamente." : "No se pudo salir del proyecto.";

            return RedirectToAction("Index", "Proyecto");
        }

        public async Task<IActionResult> MisProyectos()
        {
            var actualUser = (await _usuarioBL.SearchAsync(new Usuario { NombreUsuario = User.Identity.Name, Top_Aux = 1 }))
                                .FirstOrDefault();
            if (actualUser == null) return NotFound();

            var proyectos = await _proyectoUsuarioBL.ObtenerProyectosPorUsuarioAsync(actualUser.Id);
            return View("MisProyectos", proyectos);
        }
    }
}
