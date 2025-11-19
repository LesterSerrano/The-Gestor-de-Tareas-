using GestordeTaras.EN;
using GestordeTareas.BL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GestordeTareas.UI.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class ProyectoController : Controller
    {
        private readonly ProyectoUsuarioBL _proyectoUsuarioBL;
        private readonly ProyectoBL _proyectoBL;
        private readonly UsuarioBL _usuarioBL;
        private readonly InvitacionProyectoBL _invitacionProyectoBL;
        private readonly IEmailService _emailService;

        public ProyectoController(
            ProyectoUsuarioBL proyectoUsuarioBL,
            ProyectoBL proyectoBL,
            UsuarioBL usuarioBL,
            InvitacionProyectoBL invitacionProyectoBL,
            IEmailService emailService)
        {
            _proyectoUsuarioBL = proyectoUsuarioBL;
            _proyectoBL = proyectoBL;
            _usuarioBL = usuarioBL;
            _invitacionProyectoBL = invitacionProyectoBL;
            _emailService = emailService;
        }

        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<ActionResult> Index(string query)
        {
            List<Proyecto> lista = string.IsNullOrEmpty(query)
                ? await _proyectoBL.GetAllAsync()
                : await _proyectoBL.BuscarPorTituloOAdministradorAsync(query);

            return View(lista);
        }

        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<ActionResult> Details(int id)
        {
            var proyecto = await _proyectoBL.GetByIdAsync(new Proyecto { Id = id });
            if (proyecto == null) return NotFound();

            ViewBag.UsuariosUnidos = await _proyectoUsuarioBL.ObtenerUsuariosUnidosAsync(id);
            ViewBag.Encargado = await _proyectoUsuarioBL.ObtenerEncargadoPorProyectoAsync(id);
            ViewBag.EsEncargado = await _proyectoUsuarioBL.IsUsuarioEncargadoAsync(
                id, int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value));

            return View(proyecto);
        }

        public ActionResult Create() => PartialView("Create");

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> Create(Proyecto proyecto)
        {
            try
            {
                var actualUser = (await _usuarioBL.SearchAsync(new Usuario
                { NombreUsuario = User.Identity.Name, Top_Aux = 1 })).FirstOrDefault();
                proyecto.IdUsuario = actualUser.Id;

                string codigoAcceso;
                do
                {
                    codigoAcceso = _proyectoBL.GenerarCodigoAcceso();
                } while (await _proyectoBL.ExisteCodigoAccesoAsync(codigoAcceso));

                proyecto.CodigoAcceso = codigoAcceso;

                await _proyectoBL.CreateAsync(proyecto);
                return Json(new { success = true, message = "Proyecto creado correctamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hubo un problema al crear el proyecto: " + ex.Message });
            }
        }

        public async Task<ActionResult> Edit(int id)
        {
            var proyecto = await _proyectoBL.GetByIdAsync(new Proyecto { Id = id });
            return PartialView("Edit", proyecto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> Edit(int id, Proyecto proyecto)
        {
            try
            {
                await _proyectoBL.UpdateAsync(proyecto);
                return Json(new { success = true, message = "Proyecto actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hubo un problema al actualizar el proyecto: " + ex.Message });
            }
        }

        public async Task<ActionResult> Delete(int id)
        {
            var proyecto = await _proyectoBL.GetByIdAsync(new Proyecto { Id = id });
            return PartialView("Delete", proyecto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> Delete(int id, Proyecto proyecto)
        {
            try
            {
                await _proyectoBL.DeleteAsync(proyecto);
                return Json(new { success = true, message = "Proyecto eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hubo un problema al eliminar el proyecto: " + ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> AsignarEncargado(int idProyecto, int idUsuario)
        {
            try
            {
                bool resultado = await _proyectoUsuarioBL.AsignarEncargadoAsync(idProyecto, idUsuario);
                TempData[resultado ? "SuccessMessage" : "ErrorMessage"] =
                    resultado ? "Usuario asignado como encargado correctamente" : "Ya existe un encargado para este proyecto";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hubo un problema al asignar el encargado: " + ex.Message;
            }
            return RedirectToAction("Details", new { id = idProyecto });
        }

        // --- Métodos de invitaciones ---
        [HttpPost]
        [Authorize(Roles = "Administrador, Colaborador")]
        public async Task<IActionResult> EnviarInvitacion(InvitacionProyecto invitacion)
        {
            invitacion.FechaCreacion = DateTime.Now;
            invitacion.FechaExpiracion = DateTime.Now.AddDays(7);
            invitacion.Estado = "Pendiente";
            invitacion.Token = Guid.NewGuid().ToString();

            try
            {
                var proyecto = await _proyectoBL.GetByIdAsync(new Proyecto { Id = invitacion.IdProyecto });
                if (proyecto == null) return RedirectToAction("Details", new { id = invitacion.IdProyecto });

                int result = await _invitacionProyectoBL.EnviarInvitacionAsync(invitacion);
                if (result <= 0)
                {
                    TempData["ErrorMessage"] = result == -1
                        ? "El usuario ya está unido a este proyecto."
                        : "Ya se ha enviado una invitación a este correo electrónico.";
                    return RedirectToAction("Invitaciones", new { id = invitacion.IdProyecto });
                }

                string baseUrl = "https://localhost:7297";
                string enlaceAceptar = $"{baseUrl}/Proyecto/AceptarInvitacion?token={invitacion.Token}&decision=aceptar";
                string enlaceRechazar = $"{baseUrl}/Proyecto/AceptarInvitacion?token={invitacion.Token}&decision=rechazar";

                await _emailService.SendProjectInvitationAsync(
                    invitacion.CorreoElectronico,
                    proyecto.Titulo,
                    enlaceAceptar,
                    enlaceRechazar,
                    invitacion.FechaExpiracion
                );

                TempData["SuccessMessage"] = "Invitación enviada correctamente";
                return RedirectToAction("Invitaciones", new { id = invitacion.IdProyecto });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al enviar la invitación: " + ex.Message;
                return RedirectToAction("Invitaciones", new { id = invitacion.IdProyecto });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> AceptarInvitacion(string token, string decision)
        {
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Index");

            if (!User.Identity.IsAuthenticated)
            {
                TempData["Token"] = token;
                TempData["Decision"] = decision;
                return RedirectToAction("Login", "Usuario");
            }

            int idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            string correoUsuario = User.Identity.Name;
            var invitacion = await _invitacionProyectoBL.ObtenerPorTokenAsync(token);
            if (invitacion == null || invitacion.CorreoElectronico != correoUsuario)
            {
                TempData["ErrorMessage"] = "La invitación no existe o el correo no coincide.";
                return RedirectToAction("Index");
            }

            if (decision == "aceptar")
            {
                int result = await _invitacionProyectoBL.AceptarInvitacionAsync(token, idUsuario, correoUsuario);
                TempData[result > 0 ? "SuccessMessage" : "ErrorMessage"] = result > 0
                    ? "Has aceptado la invitación y te has unido al proyecto"
                    : "No se pudo aceptar la invitación o ya perteneces al proyecto";
            }
            else if (decision == "rechazar")
            {
                int result = await _invitacionProyectoBL.RechazarInvitacionAsync(token, idUsuario, correoUsuario);
                TempData[result > 0 ? "SuccessMessage" : "ErrorMessage"] = result > 0
                    ? "Invitación rechazada correctamente"
                    : "No se pudo rechazar la invitación";
            }

            TempData.Remove("Token");
            TempData.Remove("Decision");

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Administrador, Colaborador")]
        public async Task<IActionResult> EliminarInvitacion(int id, int idProyecto)
        {
            try
            {
                bool eliminado = await _invitacionProyectoBL.LimpiarInvitacionPorIdAsync(id);
                TempData[eliminado ? "SuccessMessage" : "InfoMessage"] =
                    eliminado ? "La invitación ha sido eliminada" : "No se encontró la invitación o no se puede eliminar";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al eliminar la invitación: " + ex.Message;
            }

            return RedirectToAction("Invitaciones", new { id = idProyecto });
        }

        [HttpGet]
        [Authorize(Roles = "Administrador, Colaborador")]
        public async Task<IActionResult> Invitaciones(int id)
        {
            try
            {
                int idUsuarioActual = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                bool esEncargado = await _proyectoUsuarioBL.IsUsuarioEncargadoAsync(id, idUsuarioActual);
                if (!esEncargado && !User.IsInRole("Administrador"))
                {
                    TempData["ErrorMessage"] = "No tienes permisos para acceder a las invitaciones de este proyecto.";
                    return RedirectToAction("Index", "Proyecto");
                }

                ViewBag.IdProyecto = id;
                ViewBag.EsEncargado = esEncargado;

                var invitaciones = await _invitacionProyectoBL.ObtenerInvitacionesPorProyectoAsync(id);
                return View(invitaciones);
            }
            catch
            {
                TempData["ErrorMessage"] = "Ocurrió un error al obtener las invitaciones";
                return View("Error");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrador, Colaborador")]
        public async Task<IActionResult> FiltrarInvitaciones(int id, string estado)
        {
            try
            {
                IEnumerable<InvitacionProyecto> invitaciones = estado == "Todos"
                    ? await _invitacionProyectoBL.ObtenerInvitacionesPorProyectoAsync(id)
                    : await _invitacionProyectoBL.ObtenerInvitacionesPorEstadoAsync(id, new List<string> { estado });

                if (!invitaciones.Any()) TempData["InfoMessage"] = "No hay invitaciones que coincidan con el filtro";

                int idUsuarioActual = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                ViewBag.EsEncargado = await _proyectoUsuarioBL.IsUsuarioEncargadoAsync(id, idUsuarioActual);
                ViewBag.IdProyecto = id;
                ViewBag.Estado = estado;

                return View("Invitaciones", invitaciones);
            }
            catch
            {
                TempData["ErrorMessage"] = "Ocurrió un error al filtrar las invitaciones";
                return View("Error");
            }
        }
    }
}
