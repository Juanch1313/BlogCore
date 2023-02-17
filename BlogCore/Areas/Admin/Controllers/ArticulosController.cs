using BlogCore.AccesoDatos.Data.Repository.IRepository;
using BlogCore.Data;
using BlogCore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BlogCore.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class ArticulosController : Controller
    {
        private readonly IUnidadDeTrabajo _unidadDeTrabajo;
        private readonly IWebHostEnvironment _hostringEnviroment;

        public ArticulosController(IUnidadDeTrabajo unidadDeTrabajo, IWebHostEnvironment hostringEnviroment)
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
            ArticuloVM articuloVM = new ArticuloVM()
            {
                Articulo = new BlogCore.Models.Articulo(),
                ListaCategorias = _unidadDeTrabajo.Categoria.GetListaCategorias()
            };

            return View(articuloVM);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Create(ArticuloVM articuloVM)
        {
            if (ModelState.IsValid)
            {
                string rutaPrincipal = _hostringEnviroment.WebRootPath;
                var archivos = HttpContext.Request.Form.Files;
                if(articuloVM.Articulo.Id == 0)
                {
                    //Nuevo articulo
                    string NombreArchivo = Guid.NewGuid().ToString();
                    var subidas = Path.Combine(rutaPrincipal, @"imagenes\articulos\");
                    var extension = Path.GetExtension(archivos[0].FileName);


                    using (var filestreams = new FileStream(Path.Combine(subidas, NombreArchivo + extension), FileMode.Create))
                    {
                        archivos[0].CopyTo(filestreams);
                    }

                    articuloVM.Articulo.UrlImagen = @"\imagenes\articulos\" + NombreArchivo + extension;
                    articuloVM.Articulo.FechaCreacion = DateTime.Now.ToString();

                    _unidadDeTrabajo.Articulo.Add(articuloVM.Articulo);
                    _unidadDeTrabajo.Save();

                    return RedirectToAction(nameof(Index));
                }
            }
            articuloVM.ListaCategorias = _unidadDeTrabajo.Categoria.GetListaCategorias();
            return View(articuloVM);
        }


        [HttpGet]
        public IActionResult Edit(int? id)
        {
            ArticuloVM articuloVM = new ArticuloVM()
            {
                Articulo = new Models.Articulo(),
                ListaCategorias = _unidadDeTrabajo.Categoria.GetListaCategorias()
            };

            if(id != null)
            {
                articuloVM.Articulo = _unidadDeTrabajo.Articulo.Get(id.GetValueOrDefault());
            }

            return View(articuloVM);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Edit(ArticuloVM articuloVM)
        {
            if (ModelState.IsValid)
            {
                string rutaPrincipal = _hostringEnviroment.WebRootPath;
                var archivos = HttpContext.Request.Form.Files;

                var articuloDesdebd = _unidadDeTrabajo.Articulo.Get(articuloVM.Articulo.Id);

                if (archivos.Count > 0)
                {
                    //Nueva imagen para el articulo
                    string NombreArchivo = Guid.NewGuid().ToString();
                    var subidas = Path.Combine(rutaPrincipal, @"imagenes\articulos\");
                    var extension = Path.GetExtension(archivos[0].FileName);

                    var nuevaExtension = Path.GetExtension(archivos[0].FileName);
                    var rutaImagen = Path.Combine(rutaPrincipal, articuloDesdebd.UrlImagen.TrimStart('\\'));


                    if (System.IO.File.Exists(rutaImagen)) System.IO.File.Delete(rutaImagen);


                    //Nuevamente subimos archivo
                    using (var filestreams = new FileStream(Path.Combine(subidas, NombreArchivo + extension), FileMode.Create))
                    {
                        archivos[0].CopyTo(filestreams);
                    }

                    articuloVM.Articulo.UrlImagen = @"\imagenes\articulos\" + NombreArchivo + extension;

                }
                else
                {
                    //Cuando la imagen ya existe y se conserva
                    articuloVM.Articulo.UrlImagen = articuloDesdebd.UrlImagen;
                }
                _unidadDeTrabajo.Articulo.Update(articuloVM.Articulo);
                _unidadDeTrabajo.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(articuloVM);
        }

        #region Llamadas a la API
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _unidadDeTrabajo.Articulo.GetAll(includeProperties: "Categoria") });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var articuloDesdeDb = _unidadDeTrabajo.Articulo.Get(id);
            var rutaDirectorioPrincipal = _hostringEnviroment.WebRootPath;

            var rutaImagen = Path.Combine(rutaDirectorioPrincipal, articuloDesdeDb.UrlImagen.TrimStart('\\'));

            if (System.IO.File.Exists(rutaImagen)) System.IO.File.Delete(rutaImagen);

             if(articuloDesdeDb == null)
            {
                return Json(new { success = false, message = "Error al borrar el artículo" });
            }

            var objFromDb = _unidadDeTrabajo.Articulo.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error al borrar la categoria" });
            }

            _unidadDeTrabajo.Articulo.Remove(articuloDesdeDb);
            _unidadDeTrabajo.Save();
            return Json(new { success = true, message = "Artículo eliminado correctamente" });
        }

        #endregion
    }
}
