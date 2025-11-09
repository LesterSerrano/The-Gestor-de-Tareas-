using GestordeTaras.EN;
using GestordeTareas.BL;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestordeTareas.UI.Controllers
{
    [Authorize(Roles = "Administrador", AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class CategoriaController : Controller
    {
        private readonly CategoriaBL _categoriaBL;

        public CategoriaController()
        {
            _categoriaBL = new CategoriaBL();
        }

        public async Task<ActionResult> Index()
        {
            var categorias = await _categoriaBL.GetAllAsync();
            return View("Index", categorias);
        }

        public async Task<ActionResult> Details(int id)
        {
            var categoria = await _categoriaBL.GetById(new Categoria { Id = id });
            return PartialView("Details", categoria);
        }

        public ActionResult Create()
        {
            return PartialView("Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Categoria categoria)
        {
            try
            {
                await _categoriaBL.CreateAsync(categoria);
                return Json(new { success = true, message = "Categoría creada correctamente." });
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return Json(new { success = false, message = $"Error al crear la categoría: {ex.Message}" });
            }
        }

        public async Task<ActionResult> Edit(int id)
        {
            var categoria = await _categoriaBL.GetById(new Categoria { Id = id });
            return PartialView("Edit", categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Categoria categoria)
        {
            try
            {
                await _categoriaBL.UpdateAsync(categoria);
                return Json(new { success = true, message = "Categoría editada correctamente." });
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return Json(new { success = false, message = $"Error al editar la categoría: {ex.Message}" });
            }
        }

        public async Task<ActionResult> Delete(int id)
        {
            var categoria = await _categoriaBL.GetById(new Categoria { Id = id });
            return PartialView("Delete", categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, Categoria categoria)
        {
            try
            {
                await _categoriaBL.DeleteAsync(categoria);
                return Json(new { success = true, message = "Categoría eliminada correctamente." });
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return Json(new { success = false, message = $"Error al eliminar la categoría: {ex.Message}" });
            }
        }
    }
}
