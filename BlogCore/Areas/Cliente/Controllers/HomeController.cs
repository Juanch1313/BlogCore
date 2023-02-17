using BlogCore.AccesoDatos.Data.Repository.IRepository;
using BlogCore.Models;
using BlogCore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BlogCore.Areas.Cliente.Controllers
{
    [Area("Cliente")]
    public class HomeController : Controller
    {
        private readonly IUnidadDeTrabajo _unidadDeTrabajo;

        public HomeController(IUnidadDeTrabajo unidadDeTrabajo)
        {
            _unidadDeTrabajo = unidadDeTrabajo;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM()
            {
                ListaSliders = _unidadDeTrabajo.Slider.GetAll(),
                ListaArticulos = _unidadDeTrabajo.Articulo.GetAll()
            };

            //Esta linea es para saber si estamos en home

            ViewBag.IsHome = true;

            return View(homeVM);
        }

        public IActionResult Details(int id)
        {
            var articuloDesdeDb = _unidadDeTrabajo.Articulo.Get(id);
            return View(articuloDesdeDb);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}