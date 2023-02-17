using BlogCore.AccesoDatos.Data.Repository.IRepository;
using BlogCore.Models;
using BlogCore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BlogCore.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class SlidersController : Controller
    {
        private readonly IUnidadDeTrabajo _unidadDeTrabajo;
        private readonly IWebHostEnvironment _hostringEnviroment;

        public SlidersController(IUnidadDeTrabajo unidadDeTrabajo, IWebHostEnvironment hostringEnviroment)
        {
            _unidadDeTrabajo = unidadDeTrabajo;
            _hostringEnviroment = hostringEnviroment;
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
        [AutoValidateAntiforgeryToken]
        public IActionResult Create(Slider slider)
        {
            if (ModelState.IsValid)
            {
                string rutaPrincipal = _hostringEnviroment.WebRootPath;
                var archivos = HttpContext.Request.Form.Files;
                if (slider.Id == 0)
                {
                    //Nuevo articulo
                    string NombreArchivo = Guid.NewGuid().ToString();
                    var subidas = Path.Combine(rutaPrincipal, @"imagenes\sliders\");
                    var extension = Path.GetExtension(archivos[0].FileName);


                    using (var filestreams = new FileStream(Path.Combine(subidas, NombreArchivo + extension), FileMode.Create))
                    {
                        archivos[0].CopyTo(filestreams);
                    }

                    slider.UrlImagen = @"\imagenes\sliders\" + NombreArchivo + extension;

                    _unidadDeTrabajo.Slider.Add(slider);
                    _unidadDeTrabajo.Save();

                    return RedirectToAction(nameof(Index));
                }
            }
            return View(slider);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var slider = new Slider();
            slider = _unidadDeTrabajo.Slider.Get(id);
            if (slider == null)
            {
                return NotFound();
            }

            return View(slider);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Slider slider)
        {
            if (ModelState.IsValid)
            {
                string rutaPrincipal = _hostringEnviroment.WebRootPath;
                var archivos = HttpContext.Request.Form.Files;

                var articuloDesdebd = _unidadDeTrabajo.Slider.Get(slider.Id);

                if(archivos.Count > 0)
                {
                    //Nueva imagen para el articulo
                    string NombreArchivo = Guid.NewGuid().ToString();
                    var subidas = Path.Combine(rutaPrincipal, @"imagenes\sliders\");
                    var extension = Path.GetExtension(archivos[0].FileName);

                    var nuevaExtension = Path.GetExtension(archivos[0].FileName);
                    var rutaImagen = Path.Combine(rutaPrincipal, articuloDesdebd.UrlImagen.TrimStart('\\'));


                    if (System.IO.File.Exists(rutaImagen)) System.IO.File.Delete(rutaImagen);


                    //Nuevamente subimos archivo
                    using (var filestreams = new FileStream(Path.Combine(subidas, NombreArchivo + extension), FileMode.Create))
                    {
                        archivos[0].CopyTo(filestreams);
                    }

                    slider.UrlImagen = @"\imagenes\sliders\" + NombreArchivo + extension;
                }
                else
                {
                    //Cuando la imagen ya existe y se conserva
                    slider.UrlImagen = articuloDesdebd.UrlImagen;
                }
                _unidadDeTrabajo.Slider.Update(slider);
                _unidadDeTrabajo.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(slider);
        }

        #region Llamadas a la API
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _unidadDeTrabajo.Slider.GetAll() });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unidadDeTrabajo.Slider.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error al borrar el slider" });
            }

            _unidadDeTrabajo.Slider.Remove(objFromDb);
            _unidadDeTrabajo.Save();
            return Json(new { success = true, message = "Slider eliminado correctamente" });
        }
        #endregion
    }
}
