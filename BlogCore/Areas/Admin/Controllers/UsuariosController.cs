using BlogCore.AccesoDatos.Data.Repository.IRepository;
using BlogCore.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;

namespace BlogCore.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class UsuariosController : Controller
    {
        private readonly IUnidadDeTrabajo _unidadDeTrabajo;

        public UsuariosController(IUnidadDeTrabajo unidadDeTrabajo, ApplicationDbContext context)
        {
            _unidadDeTrabajo = unidadDeTrabajo;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var usuarioActual = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            return View(_unidadDeTrabajo.Usuario.GetAll(u => u.Id != usuarioActual.Value));
        }
        
        [HttpGet]
        public IActionResult Bloquear(string Id)
        {
            if (Id == null) return NotFound();
            _unidadDeTrabajo.Usuario.BloquearUsuario(Id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Desbloquear(string Id)
        {
            if (Id == null) return NotFound();
            _unidadDeTrabajo.Usuario.DesbloquearUsuario(Id);
            return RedirectToAction(nameof(Index));
        }
    }
}
