using GestordeTareas.BL;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GestordeTaras.EN;

namespace GestordeTareas.UI.Controllers
{
    [Authorize(Roles = "Administrador", AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class EstadoTareaController : Controller
    {
        private readonly EstadoTareaBL _estadoTareaBL;

        public EstadoTareaController(EstadoTareaBL estadoTareaBL)
        {
            _estadoTareaBL = estadoTareaBL;
        }

        public async Task<ActionResult> Index()
        {
            var lista = await _estadoTareaBL.GetAllAsync();
            return View("Index", lista);
        }

        public async Task<ActionResult> Details(int id)
        {
            var estadoTarea = await _estadoTareaBL.GetByIdAsync(new EstadoTarea { Id = id });
            return PartialView("Details", estadoTarea);
        }

        public ActionResult Create()
        {
            return PartialView("Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(EstadoTarea estadoTarea)
        {
            try
            {
                await _estadoTareaBL.CreateAsync(estadoTarea);
                return Json(new { success = true, message = "Estado creado correctamente." });
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return Json(new { success = false, message = $"Error al crear el estado: {ex.Message}" });
            }
        }

        public async Task<ActionResult> Edit(int id)
        {
            var estadoTarea = await _estadoTareaBL.GetByIdAsync(new EstadoTarea { Id = id });
            return PartialView("Edit", estadoTarea);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, EstadoTarea estadoTarea)
        {
            try
            {
                await _estadoTareaBL.UpdateAsync(estadoTarea);
                return Json(new { success = true, message = "Estado editado correctamente." });
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return Json(new { success = false, message = $"Error al editar el estado: {ex.Message}" });
            }
        }

        public async Task<ActionResult> Delete(int id)
        {
            var estadoTarea = await _estadoTareaBL.GetByIdAsync(new EstadoTarea { Id = id });
            return PartialView("Delete", estadoTarea);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, EstadoTarea estadoTarea)
        {
            try
            {
                await _estadoTareaBL.DeleteAsync(estadoTarea);
                return Json(new { success = true, message = "Estado eliminado correctamente." });
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return Json(new { success = false, message = $"Error al eliminar el estado: {ex.Message}" });
            }
        }
    }
}
