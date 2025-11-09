using GestordeTaras.EN;
using GestordeTareas.BL;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GestordeTareas.UI.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class CommentController : Controller
    {
        private readonly CommentBL _commentBL;
        private readonly ProyectoBL _proyectoBL;
        private readonly UsuarioBL _usuarioBL;
        private readonly ProyectoUsuarioBL _proyectoUsuarioBL;

        public CommentController()
        {
            _commentBL = new CommentBL();
            _proyectoBL = new ProyectoBL();
            _usuarioBL = new UsuarioBL();
            _proyectoUsuarioBL = new ProyectoUsuarioBL();
        }

        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<ActionResult> Index(int idProyecto)
        {
            int idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (!User.IsInRole("Administrador"))
            {
                var usuariosUnidos = await _proyectoUsuarioBL.ObtenerUsuariosUnidosAsync(idProyecto);
                if (!usuariosUnidos.Any(u => u.Id == idUsuario))
                {
                    TempData["ErrorMessage"] = "No estás unido a este proyecto, no puedes ver los comentarios.";
                    return RedirectToAction("Index", "Proyecto");
                }
            }

            var comentarios = await _commentBL.ObtenerComentariosPorProyectoAsync(idProyecto);
            ViewBag.IdProyecto = idProyecto;
            return View(comentarios);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Create(int idProyecto, string contenido)
        {
            int idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (!User.IsInRole("Administrador"))
            {
                var usuariosUnidos = await _proyectoUsuarioBL.ObtenerUsuariosUnidosAsync(idProyecto);
                if (!usuariosUnidos.Any(u => u.Id == idUsuario))
                {
                    TempData["ErrorMessage"] = "No estás unido a este proyecto, no puedes crear comentarios.";
                    return RedirectToAction("Index", "Proyecto");
                }
            }

            if (!string.IsNullOrEmpty(contenido))
            {
                int result = await _commentBL.CrearComentarioAsync(idProyecto, idUsuario, contenido);
                if (result > 0) return RedirectToAction("Index", new { idProyecto });
                TempData["ErrorMessage"] = "Hubo un error al crear el comentario";
            }
            else
            {
                TempData["ErrorMessage"] = "El contenido del comentario no puede estar vacío";
            }

            return RedirectToAction("Index", new { idProyecto });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Delete(int idComentario, int idProyecto)
        {
            int idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            int result = await _commentBL.EliminarComentarioAsync(idComentario, idUsuario);

            TempData["InfoMessage"] = result == 1 ? "Comentario eliminado" : null;
            TempData["ErrorMessage"] = result switch
            {
                -1 => "No puedes eliminar un comentario que no te pertenece",
                -2 => "El tiempo para eliminar el comentario ha expirado (más de 15 minutos)",
                0 => "Hubo un error al eliminar el comentario",
                _ => TempData["ErrorMessage"]
            };

            return RedirectToAction("Index", new { idProyecto });
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerComentarios(int idProyecto)
        {
            var comentarios = await _commentBL.ObtenerComentariosPorProyectoAsync(idProyecto);
            return PartialView("_Comentarios", comentarios);
        }
    }
}
