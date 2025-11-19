using GestordeTareas.BL;
using GestordeTareas.UI.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GestordeTareas.UI.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UsuarioBL _usuarioBL;
        private readonly ProyectoBL _proyectoBL;
        private readonly TareaBL _tareaBL;
        private readonly ProyectoUsuarioBL _proyectoUsuarioBL;

        public HomeController(ILogger<HomeController> logger, UsuarioBL usuarioBL, ProyectoBL proyectoBL, TareaBL tareaBL, ProyectoUsuarioBL proyectoUsuarioBL)
        {
            _logger = logger;
            _usuarioBL = usuarioBL;
            _proyectoBL = proyectoBL;
            _tareaBL = tareaBL;
            _proyectoUsuarioBL = proyectoUsuarioBL;
        }

        public async Task<IActionResult> Index()
        {
            var usuarios = await _usuarioBL.GetAllAsync();
            var proyectos = await _proyectoBL.GetAllAsync();
            var tareas = await _tareaBL.GetAllAsync();
            var usuariosPorProyecto = await _proyectoUsuarioBL.ObtenerTodosAsync();

            ViewBag.TotalUsuarios = usuarios.Count;
            ViewBag.TotalProyectos = proyectos.Count;
            ViewBag.TotalTareas = tareas.Count;

            var tareasPorProyecto = proyectos.ToDictionary(
                p => p.Titulo,
                p => new
                {
                    TotalTareas = tareas.Count(t => t.IdProyecto == p.Id),
                    Pendientes = tareas.Count(t => t.IdProyecto == p.Id && t.EstadoTarea != null && t.EstadoTarea.Nombre == "Pendiente"),
                    EnProceso = tareas.Count(t => t.IdProyecto == p.Id && t.EstadoTarea != null && t.EstadoTarea.Nombre == "En Proceso"),
                    Finalizadas = tareas.Count(t => t.IdProyecto == p.Id && t.EstadoTarea != null && t.EstadoTarea.Nombre == "Finalizada")
                }
            );

            var progresoPorProyecto = proyectos.ToDictionary(
                p => p.Titulo,
                p =>
                {
                    var totalTareas = tareas.Count(t => t.IdProyecto == p.Id);
                    var tareasFinalizadas = tareas.Count(t => t.IdProyecto == p.Id && t.EstadoTarea != null && t.EstadoTarea.Nombre == "Finalizada");
                    return totalTareas == 0 ? 0 : (int)((double)tareasFinalizadas / totalTareas * 100);
                }
            );

            var usuariosPorProyectoDiccionario = proyectos.ToDictionary(
                p => p.Titulo,
                p => usuariosPorProyecto.Count(pu => pu.IdProyecto == p.Id)
            );

            ViewBag.TareasPorProyecto = tareasPorProyecto;
            ViewBag.ProgresoPorProyecto = progresoPorProyecto;
            ViewBag.UsuariosPorProyecto = usuariosPorProyectoDiccionario;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
