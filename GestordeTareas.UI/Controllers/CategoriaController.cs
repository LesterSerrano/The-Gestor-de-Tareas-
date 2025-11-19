using GestordeTaras.EN;
using GestordeTareas.BL;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using GestordeTareas.UI.DTOs.CategoriaDTOs;


namespace GestordeTareas.UI.Controllers
{
    [Authorize(Roles = "Administrador", AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class CategoriaController : Controller
    {
        private readonly CategoriaBL _categoriaBL;
        private readonly IMapper _mapper;

<<<<<<< HEAD
        public CategoriaController(CategoriaBL categoriaBL, IMapper mapper)
        {
            _categoriaBL = categoriaBL;
            _mapper = mapper;
=======
        public CategoriaController(CategoriaBL categoriaBL)
        {
            _categoriaBL = categoriaBL;
>>>>>>> 6f3904e1ef769397f15b3f638d17e031b23152fb
        }

        // GET: /Categoria
        public async Task<IActionResult> Index()
        {
            var categoriasEN = await _categoriaBL.GetAllAsync();
            var categoriasDTO = _mapper.Map<IEnumerable<CategoriaReadDTO>>(categoriasEN);

            return View("Index", categoriasDTO);
        }

        // GET: /Categoria/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
<<<<<<< HEAD
            var categoriaEN = await _categoriaBL.GetByIdAsync(id);
            var categoriaDTO = _mapper.Map<CategoriaReadDTO>(categoriaEN);

            return PartialView("Details", categoriaDTO);
=======
            var categoria = await _categoriaBL.GetByIdAsync(new Categoria { Id = id });
            return PartialView("Details", categoria);
>>>>>>> 6f3904e1ef769397f15b3f638d17e031b23152fb
        }

        // GET: /Categoria/Create
        public IActionResult Create()
        {
            return PartialView("Create");
        }

        // POST: /Categoria/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoriaCreateDto dto)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Datos inválidos" });

            try
            {
                var categoriaEN = _mapper.Map<Categoria>(dto);
                await _categoriaBL.CreateAsync(categoriaEN);

                return Json(new { success = true, message = "Categoría creada correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al crear la categoría: {ex.Message}" });
            }
        }

        // GET: /Categoria/Edit/id
        public async Task<IActionResult> Edit(int id)
        {
<<<<<<< HEAD
            var categoriaEN = await _categoriaBL.GetByIdAsync(id);
            var dto = _mapper.Map<CategoriaUpdateDto>(categoriaEN);

            return PartialView("Edit", dto);
=======
            var categoria = await _categoriaBL.GetByIdAsync(new Categoria { Id = id });
            return PartialView("Edit", categoria);
>>>>>>> 6f3904e1ef769397f15b3f638d17e031b23152fb
        }

        // POST: /Categoria/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoriaUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Datos inválidos" });

            try
            {
                var categoriaEN = _mapper.Map<Categoria>(dto);
                await _categoriaBL.UpdateAsync(categoriaEN);

                return Json(new { success = true, message = "Categoría editada correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al editar la categoría: {ex.Message}" });
            }
        }

        // GET: /Categoria/Delete/id
        public async Task<IActionResult> Delete(int id)
        {
<<<<<<< HEAD
            var categoriaEN = await _categoriaBL.GetByIdAsync(id);
            var dto = _mapper.Map<CategoriaReadDTO>(categoriaEN);

            return PartialView("Delete", dto);
=======
            var categoria = await _categoriaBL.GetByIdAsync(new Categoria { Id = id });
            return PartialView("Delete", categoria);
>>>>>>> 6f3904e1ef769397f15b3f638d17e031b23152fb
        }

        // POST: /Categoria/DeleteConfirmed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _categoriaBL.DeleteAsync(id);
                return Json(new { success = true, message = "Categoría eliminada correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al eliminar la categoría: {ex.Message}" });
            }
        }
    }
}

