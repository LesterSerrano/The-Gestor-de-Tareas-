using GestordeTaras.EN;
using GestordeTareas.BL;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestordeTareas.UI.Controllers
{
    [Authorize(Roles = "Administrador", AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class CargoController : Controller
    {
        private readonly CargoBL _cargoBL;

        public CargoController(CargoBL cargoBL)
        {
            _cargoBL = cargoBL;
        }

        public async Task<ActionResult> Index()
        {
            var cargos = await _cargoBL.GetAllAsync();
            return View("Index", cargos);
        }

        public async Task<ActionResult> Details(int id)
        {
            var cargo = await _cargoBL.GetById(new Cargo { Id = id });
            return PartialView("Details", cargo);
        }

        public ActionResult Create()
        {
            return PartialView("Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Cargo cargo)
        {
            try
            {
                await _cargoBL.CreateAsync(cargo);
                return Json(new { success = true, message = "Cargo creado correctamente." });
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return Json(new { success = false, message = $"Error al crear el cargo: {ex.Message}" });
            }
        }

        public async Task<ActionResult> Edit(int id)
        {
            var cargo = await _cargoBL.GetById(new Cargo { Id = id });
            return PartialView("Edit", cargo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Cargo cargo)
        {
            try
            {
                await _cargoBL.UpdateAsync(cargo);
                return Json(new { success = true, message = "Cargo editado correctamente." });
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return Json(new { success = false, message = $"Error al editar el cargo: {ex.Message}" });
            }
        }

        public async Task<ActionResult> Delete(int id)
        {
            var cargo = await _cargoBL.GetById(new Cargo { Id = id });
            return PartialView("Delete", cargo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, Cargo cargo)
        {
            try
            {
                await _cargoBL.DeleteAsync(cargo);
                return Json(new { success = true, message = "Cargo eliminado correctamente." });
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return Json(new { success = false, message = $"Error al eliminar el cargo: {ex.Message}" });
            }
        }
    }
}
