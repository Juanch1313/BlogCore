using BlogCore.AccesoDatos.Data.Repository.IRepository;
using BlogCore.Data;
using BlogCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogCore.Areas.Admin.Controllers
{
    [Authorize(Roles ="Admin")]
    [Area("Admin")]
    public class CategoriasController : Controller
    {
        private readonly IUnidadDeTrabajo _unidadDeTrabajo;

        public CategoriasController(IUnidadDeTrabajo unidadDeTrabajo, ApplicationDbContext context)
        {
            _unidadDeTrabajo = unidadDeTrabajo;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                _unidadDeTrabajo.Categoria.Add(categoria);
                _unidadDeTrabajo.Save();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var categoria = new Categoria();
            categoria = _unidadDeTrabajo.Categoria.Get(id);
            if (categoria == null)
            {
                return NotFound();
            }
            return View(categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                _unidadDeTrabajo.Categoria.Update(categoria);
                _unidadDeTrabajo.Save();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }


        #region Llamadas a la API
        [HttpGet]
        public IActionResult GetAll() 
        {
            return Json(new {data = _unidadDeTrabajo.Categoria.GetAll()});
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unidadDeTrabajo.Categoria.Get(id);
            if(objFromDb == null)
            {
                return Json(new { success = false, message = "Error al borrar la categoria" });
            }

            _unidadDeTrabajo.Categoria.Remove(objFromDb);
            _unidadDeTrabajo.Save();
            return Json(new { success = true, message = "Categoría eliminada correctamente" });
        }

        #endregion
    }
}
