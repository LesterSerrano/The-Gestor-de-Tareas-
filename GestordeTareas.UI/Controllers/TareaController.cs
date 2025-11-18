using GestordeTaras.EN;
using GestordeTareas.BL;
using GestordeTareas.DAL;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GestordeTareas.UI.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class TareaController : Controller
    {
        private readonly TareaBL _tareaBL;
        private readonly CategoriaBL _categoriaBL;
        private readonly PrioridadBL _prioridadBL;
        private readonly EstadoTareaBL _estadoTareaBL;
        private readonly ProyectoBL _proyectoBL;
        private readonly UsuarioBL _usuarioBL;
        private readonly ProyectoUsuarioBL _proyectoUsuarioBL;
        //private readonly ElegirTareaBL _elegirTareaBL;

        public TareaController(
            TareaBL tareaBL,
            CategoriaBL categoriaBL,
            PrioridadBL prioridadBL,
            EstadoTareaBL estadoTareaBL,
            ProyectoBL proyectoBL,
            UsuarioBL usuarioBL,
            ProyectoUsuarioBL proyectoUsuarioBL)
            //ElegirTareaBL elegirTareaBL)
        {
            _tareaBL = tareaBL;
            _categoriaBL = categoriaBL;
            _prioridadBL = prioridadBL;
            _estadoTareaBL = estadoTareaBL;
            _proyectoBL = proyectoBL;
            _usuarioBL = usuarioBL;
            _proyectoUsuarioBL = proyectoUsuarioBL;
            //_elegirTareaBL = elegirTareaBL;
        }

        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<ActionResult> Index(int proyectoId)
        {
            if (!await VerificarAcceso(proyectoId))
            {
                TempData["ErrorMessage"] = "No tienes acceso a este proyecto";
                return RedirectToAction("Index", "Proyecto");
            }

            var tareas = await _tareaBL.GetTareasByProyectoIdAsync(proyectoId);
            ViewBag.ProyectoId = proyectoId;

            int idUsuarioActual = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            ViewBag.EsEncargado = await _proyectoUsuarioBL.IsUsuarioEncargadoAsync(proyectoId, idUsuarioActual);

            return View(tareas);
        }

        public async Task<ActionResult> Details(int id)
        {
            var tarea = await _tareaBL.GetByIdAsync(new Tarea { Id = id });
            return PartialView("Details", tarea);
        }

        public async Task<ActionResult> Create(int idProyecto)
        {
            await LoadDropDownListsAsync();
            ViewBag.ProyectoId = idProyecto;
            ViewBag.EstadoPendienteId = await _estadoTareaBL.GetEstadoPendienteIdAsync();
            return PartialView("Create", new Tarea { IdProyecto = idProyecto });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Colaborador")]
        public async Task<ActionResult> Create(Tarea tarea, int idProyecto)
        {
            if (!await TienePermisoParaProyectoAsync(tarea.IdProyecto))
            {
                return Json(new { success = false, message = "No tienes permisos para realizar cambios" });
            }

            try
            {
                tarea.IdProyecto = idProyecto;
                tarea.FechaCreacion = DateTime.Now;
                tarea.IdEstadoTarea = await _estadoTareaBL.GetEstadoPendienteIdAsync();

                await _tareaBL.CreateAsync(tarea);
                TempData["SuccessMessage"] = "Tarea creada correctamente.";
                return Json(new { success = true, message = "Tarea creada correctamente", id = idProyecto });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al crear la tarea: " + ex.Message });
            }
        }


        private async Task LoadDropDownListsAsync()
        {
            ViewBag.Categorias = new SelectList(await _categoriaBL.GetAllAsync(), "Id", "Nombre");
            ViewBag.Prioridades = new SelectList(await _prioridadBL.GetAllAsync(), "Id", "Nombre");
            ViewBag.EstadosTarea = new SelectList(await _estadoTareaBL.GetAllAsync(), "Id", "Nombre");
            ViewBag.Proyectos = new SelectList(await _proyectoBL.GetAllAsync(), "Id", "Titulo");
        }

        public async Task<ActionResult> Edit(int id)
        {
            await LoadDropDownListsAsync();
            return PartialView("Edit", await _tareaBL.GetByIdAsync(new Tarea { Id = id }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Colaborador")]
        public async Task<ActionResult> Edit(int id, Tarea tarea)
        {
            if (!await TienePermisoParaProyectoAsync(tarea.IdProyecto))
            {
                return Json(new { success = false, message = "No tienes permisos para realizar cambios" });
            }

            try
            {
                await _tareaBL.UpdateAsync(tarea);
                TempData["SuccessMessage"] = "Tarea modificada correctamente.";
                return Json(new { success = true, message = "Tarea modificada correctamente", id = tarea.IdProyecto });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al modificar la tarea: {ex.Message}" });
            }
        }


        public async Task<ActionResult> Delete(int id)
        {
            return PartialView("Delete", await _tareaBL.GetByIdAsync(new Tarea { Id = id }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Colaborador")]
        public async Task<ActionResult> Delete(int id, Tarea tarea)
        {
            var tareaObtenida = await _tareaBL.GetByIdAsync(new Tarea { Id = id });
            if (tareaObtenida == null) return NotFound("Tarea no encontrada");

            int idUsuario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (!User.IsInRole("Administrador") &&
                !await _proyectoUsuarioBL.IsUsuarioEncargadoAsync(tareaObtenida.IdProyecto, idUsuario))
            {
                TempData["ErrorMessage"] = "No tienes permisos para realizar cambios";
                return RedirectToAction("Index");
            }

            try
            {
                await _tareaBL.DeleteAsync(tareaObtenida);
                TempData["SuccessMessage"] = "Tarea eliminada correctamente";
                return RedirectToAction("Index", new { proyectoId = tareaObtenida.IdProyecto });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al eliminar la tarea: {ex.Message}";
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("Tarea/update-state")]
        public async Task<IActionResult> ActualizarEstadoTarea([FromBody] TareaUpdateModel model)
        {
            var resultado = await _tareaBL.ActualizarEstadoTareaConValidacionAsync(model.IdTarea, model.IdEstadoTarea);

            if (!resultado.success) return BadRequest(resultado.mensaje);

            return Ok(new { nombreEstado = resultado.nombreEstado });
        }

        private async Task<bool> VerificarAcceso(int idProyecto)
        {
            var actualUser = (await _usuarioBL.SearchAsync(new Usuario { NombreUsuario = User.Identity.Name, Top_Aux = 1 }))
                                .FirstOrDefault();
            if (actualUser == null) return false;

            if (User.IsInRole("Administrador")) return true;

            var usuariosUnidos = await _proyectoUsuarioBL.ObtenerUsuariosUnidosAsync(idProyecto);
            return usuariosUnidos.Any(u => u.Id == actualUser.Id);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ElegirTarea(int idTarea)
        //{
        //    var actualUser = (await _usuarioBL.SearchAsync(new Usuario { NombreUsuario = User.Identity.Name, Top_Aux = 1 }))
        //                        .FirstOrDefault();

        //    if (actualUser == null) { TempData["ErrorMessage"] = "Usuario no encontrado"; return RedirectToAction("Index"); }

        //    var tarea = await _tareaBL.GetByIdAsync(new Tarea { Id = idTarea });
        //    if (tarea == null) { TempData["ErrorMessage"] = "Tarea no encontrada"; return RedirectToAction("Index"); }

        //    if (tarea.EstadoTarea.Nombre != "Pendiente")
        //    {
        //        TempData["ErrorMessage"] = "La tarea no está en Disponible";
        //        return RedirectToAction("Index", new { proyectoId = tarea.IdProyecto });
        //    }

        //    bool resultado = await _elegirTareaBL.ElegirTareaAsync(idTarea, actualUser.Id, tarea.IdProyecto);
        //    if (resultado) await _tareaBL.ActualizarEstadoTareaAsync(idTarea, 2); // "En Proceso"

        //    TempData[resultado ? "SuccessMessage" : "ErrorMessage"] =
        //        resultado ? "Tarea elegida correctamente" : "No se pudo elegir la tarea";

        //    return RedirectToAction("Index", new { proyectoId = tarea.IdProyecto });
        //}

        private async Task<bool> TienePermisoParaProyectoAsync(int idProyecto)
        {
            // Obtiene el ID del usuario actual
            int idUsuario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Los administradores siempre tienen permiso
            if (User.IsInRole("Administrador"))
                return true;

            // Verifica si el usuario es encargado del proyecto
            return await _proyectoUsuarioBL.IsUsuarioEncargadoAsync(idProyecto, idUsuario);
        }

    }
}
