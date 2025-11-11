using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GestordeTareas.BL;
using GestordeTaras.EN;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using GestordeTareas.UI.Helpers;
using GestordeTareas.DAL;

namespace GestordeTareas.UI.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class UsuarioController : Controller
    {
        private readonly UsuarioBL _usuarioBL;
        private readonly CargoBL _cargoBL;
        private readonly IEmailService _emailService;

        public UsuarioController(UsuarioBL usuarioBL, CargoBL cargoBL, IEmailService emailService)
        {
            _usuarioBL = usuarioBL;
            _cargoBL = cargoBL;
            _emailService = emailService;
        }

        #region Helpers

        private async Task LoadDropDownListsAsync()
        {
            var cargos = await _cargoBL.GetAllAsync();
            ViewBag.Cargos = new SelectList(cargos, "Id", "Nombre");
        }

        private async Task ActualizarClaimsUsuario(Usuario usuario)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;

            claimsIdentity.RemoveClaim(claimsIdentity.FindFirst(ClaimTypes.GivenName));
            claimsIdentity.RemoveClaim(claimsIdentity.FindFirst(ClaimTypes.Surname));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.GivenName, usuario.Nombre));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Surname, usuario.Apellido));

            claimsIdentity.RemoveClaim(claimsIdentity.FindFirst("FotoPerfil"));
            claimsIdentity.AddClaim(new Claim("FotoPerfil", usuario.FotoPerfil ?? "/img/usuario.png"));

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
        }

        private bool ValidarContrasena(string nueva, string confirmar, out string mensaje)
        {
            if (string.IsNullOrWhiteSpace(nueva) || string.IsNullOrWhiteSpace(confirmar))
            {
                mensaje = "Por favor, ingresa ambas contraseñas"; return false;
            }
            if (nueva != confirmar)
            {
                mensaje = "Las contraseñas no coinciden"; return false;
            }
            if (nueva.Length < 8)
            {
                mensaje = "La contraseña debe tener al menos 8 caracteres"; return false;
            }
            mensaje = null;
            return true;
        }

        #endregion

        #region Vistas Principales

        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> Index(string query = "", string filter = "NombreUsuario", int top = 10)
        {
            if (top <= 0) top = 10;
            var user = new Usuario { Top_Aux = top };

            var lista = await _usuarioBL.SearchIncludeRoleAsync(user, query, filter);
            lista = lista.OrderBy(u => u.Id).ToList();

            ViewBag.Top = top;
            ViewBag.Roles = await _cargoBL.GetAllAsync();

            return View("Index", lista);
        }

        [Authorize]
        public async Task<ActionResult> Perfil()
        {
            try
            {
                await LoadDropDownListsAsync();

                var users = await _usuarioBL.SearchAsync(new Usuario { NombreUsuario = User.Identity.Name, Top_Aux = 1 });
                var actualUser = users.FirstOrDefault();
                if (actualUser == null) return NotFound();

                ViewBag.NombreUsuario = $"{actualUser.Nombre} {actualUser.Apellido}";

                var usuario = await _usuarioBL.GetByIdAsync(new Usuario { Id = actualUser.Id });
                return View(usuario);
            }
            catch
            {
                TempData["ErrorMessage"] = "Ocurrió un error al cargar la información del usuario";
                return View();
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            ViewBag.Url = returnUrl;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Usuario user, string returnUrl = null)
        {
            try
            {
                var userDb = await _usuarioBL.GetByNombreUsuarioAsync(user);
                if (userDb == null)
                {
                    TempData["ErrorMessage"] = "El correo electrónico ingresado no existe";
                    return View(new Usuario { NombreUsuario = user.NombreUsuario });
                }

                if (userDb.Status != (byte)User_Status.ACTIVO)
                {
                    TempData["ErrorMessage"] = "Tu cuenta está inactiva. Contacta al administrador.";
                    return View(new Usuario { NombreUsuario = user.NombreUsuario });
                }

                if (userDb.Pass != UsuarioDAL.HashMD5(user.Pass))
                {
                    TempData["ErrorMessage"] = "Contraseña incorrecta";
                    return View(new Usuario { NombreUsuario = user.NombreUsuario });
                }

                var fotoPerfil = string.IsNullOrEmpty(userDb.FotoPerfil) ? "/img/usuario.png" : userDb.FotoPerfil;
                userDb.Cargo = await _cargoBL.GetById(new Cargo { Id = userDb.IdCargo });

                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, userDb.NombreUsuario),
                    new Claim(ClaimTypes.Role, userDb.Cargo.Nombre),
                    new Claim(ClaimTypes.GivenName, userDb.Nombre),
                    new Claim(ClaimTypes.Surname, userDb.Apellido),
                    new Claim(ClaimTypes.NameIdentifier, userDb.Id.ToString()),
                    new Claim("FotoPerfil", fotoPerfil)
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));

                if (!string.IsNullOrWhiteSpace(returnUrl)) return Redirect(returnUrl);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(new Usuario { NombreUsuario = user.NombreUsuario });
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Usuario");
        }

        #endregion

        #region CRUD Usuarios

        [AllowAnonymous]
        public async Task<IActionResult> Create()
        {
            await LoadDropDownListsAsync();
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Usuario usuario, IFormFile fotoPerfil)
        {
            try
            {
                if (string.IsNullOrEmpty(usuario.Pass) || usuario.Pass.Length < 8)
                {
                    TempData["ErrorMessage"] = "La contraseña debe tener al menos 8 caracteres";
                    return View(usuario);
                }

                usuario.Status = (byte)User_Status.ACTIVO;

                if (fotoPerfil != null && fotoPerfil.Length > 0)
                {
                    usuario.FotoPerfil = await ImageHelper.SubirArchivo(fotoPerfil.OpenReadStream(), fotoPerfil.FileName);
                }

                if (User.IsInRole("Administrador"))
                {
                    await _usuarioBL.Create(usuario);
                    TempData["SuccessMessage"] = "Usuario creado correctamente";
                    return RedirectToAction(nameof(Index));
                }

                var cargoColaboradorId = await CargoDAL.GetCargoColaboradorIdAsync();
                usuario.IdCargo = cargoColaboradorId;

                await _usuarioBL.Create(usuario);
                TempData["SuccessMessage"] = "Usuario creado correctamente. Por favor, inicia sesión";
                return RedirectToAction(nameof(Login));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                await LoadDropDownListsAsync();
                return View(usuario);
            }
        }

        [Authorize]
        public async Task<ActionResult> Details(int id)
        {
            var usuario = await _usuarioBL.GetByIdAsync(new Usuario { Id = id });
            if (usuario == null) return NotFound();

            // Nota: La vista a devolver debe ser una 'Details.cshtml'
            return PartialView("Details", usuario);
        }

        public async Task<ActionResult> Edit(int id)
        {
            var usuario = await _usuarioBL.GetByIdAsync(new Usuario { Id = id });
            if (usuario == null) return NotFound();

            await LoadDropDownListsAsync();
            return PartialView("Edit", usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> Edit(int id, Usuario usuario)
        {
            try
            {
                var existingUser = await _usuarioBL.GetByIdAsync(new Usuario { Id = id });
                if (existingUser == null) return Json(new { success = false, message = "Usuario no encontrado" });

                usuario.Pass = existingUser.Pass;
                await _usuarioBL.Update(usuario);

                return Json(new { success = true, message = "Usuario actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditOwn(Usuario usuario, string currentPassword, IFormFile fotoPerfil)
        {
            try
            {
                var existingUser = await _usuarioBL.GetByIdAsync(usuario);
                if (existingUser == null) return RedirectToAction("Perfil");

                if (!string.IsNullOrEmpty(usuario.Pass))
                {
                    if (UsuarioDAL.HashMD5(currentPassword) != existingUser.Pass)
                    {
                        TempData["ErrorMessage"] = "Contraseña actual incorrecta";
                        return RedirectToAction("Perfil");
                    }

                    if (usuario.Pass != usuario.ConfirmarPass)
                    {
                        TempData["ErrorMessage"] = "La nueva contraseña y la confirmación no coinciden";
                        return RedirectToAction("Perfil");
                    }

                    existingUser.Pass = UsuarioDAL.HashMD5(usuario.Pass);
                }

                existingUser.Nombre = usuario.Nombre;
                existingUser.Apellido = usuario.Apellido;
                existingUser.Telefono = usuario.Telefono;
                existingUser.FechaNacimiento = usuario.FechaNacimiento;
                existingUser.NombreUsuario = usuario.NombreUsuario;

                if (fotoPerfil != null && fotoPerfil.Length > 0)
                {
                    existingUser.FotoPerfil = await ImageHelper.SubirArchivo(fotoPerfil.OpenReadStream(), fotoPerfil.FileName);
                }

                await _usuarioBL.Update(existingUser);
                await ActualizarClaimsUsuario(existingUser);

                TempData["SuccessMessage"] = "Perfil actualizado correctamente";
                return RedirectToAction("Perfil");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Perfil");
            }
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            var usuario = await _usuarioBL.GetByIdAsync(new Usuario { Id = id });

            if (usuario == null)
            {
                return NotFound();
            }

            return PartialView("Delete", usuario);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, Usuario usuario)
        {
            try
            {
                await _usuarioBL.Delete(usuario);
                return Json(new { success = true, message = "Usuario eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteOwn()
        {
            try
            {
                var nombreUsuario = User.Identity.Name;
                var usuarioDB = await _usuarioBL.GetByNombreUsuarioAsync(new Usuario { NombreUsuario = nombreUsuario });

                if (usuarioDB == null) return RedirectToAction("Perfil");

                await _usuarioBL.Delete(usuarioDB);
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                TempData["SuccessMessage"] = "Cuenta eliminada correctamente";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Perfil");
            }
        }

        #endregion
    }
}
