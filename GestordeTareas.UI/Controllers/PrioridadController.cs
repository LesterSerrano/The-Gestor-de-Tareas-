using GestordeTaras.EN;
using GestordeTareas.BL;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestordeTareas.UI.Controllers
{
    [Authorize(Roles = "Administrador", AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class PrioridadController : Controller
    {
        private readonly PrioridadBL _prioridadBL;

        public PrioridadController()
        {
            _prioridadBL = new PrioridadBL();
        }

        public async Task<ActionResult> Index()
        {
            var lista = await _prioridadBL.GetAllAsync();
            return View(lista);
        }

        public async Task<ActionResult> DetailsPartial(int id)
        {
            var prioridad = await _prioridadBL.GetById(new Prioridad { Id = id });
            return PartialView("Details", prioridad);
        }

        public ActionResult Create()
        {
            return PartialView("Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Prioridad prioridad)
        {
            try
            {
                await _prioridadBL.CreateAsync(prioridad);
                return Json(new { success = true, message = "Prioridad creada correctamente." });
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return Json(new { success = false, message = $"Error al crear la prioridad: {ex.Message}" });
            }
        }

        public async Task<ActionResult> Edit(int id)
        {
            var prioridad = await _prioridadBL.GetById(new Prioridad { Id = id });
            return PartialView("Edit", prioridad);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Prioridad prioridad)
        {
            try
            {
                await _prioridadBL.UpdateAsync(prioridad);
                return Json(new { success = true, message = "Prioridad editada correctamente." });
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return Json(new { success = false, message = $"Error al editar la prioridad: {ex.Message}" });
            }
        }

        public async Task<ActionResult> Delete(int id)
        {
            var prioridad = await _prioridadBL.GetById(new Prioridad { Id = id });
            return PartialView("Delete", prioridad);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, Prioridad prioridad)
        {
            try
            {
                await _prioridadBL.DeleteAsync(prioridad);
                return Json(new { success = true, message = "Prioridad eliminada correctamente." });
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return Json(new { success = false, message = $"Error al eliminar la prioridad: {ex.Message}" });
            }
        }
    }
}
