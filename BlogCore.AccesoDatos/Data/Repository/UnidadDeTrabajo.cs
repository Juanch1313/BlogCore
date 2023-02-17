﻿using BlogCore.AccesoDatos.Data.Repository.IRepository;
using BlogCore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogCore.AccesoDatos.Data.Repository
{
    public class UnidadDeTrabajo : IUnidadDeTrabajo
    {
        private readonly ApplicationDbContext _db;

        public ICategoriaRepository Categoria { get; private set; }

        public IArticuloRepository Articulo { get; private set; }

        public ISliderRepository Slider { get; private set; }
        public IUsuarioRepository Usuario { get; private set; }

        public UnidadDeTrabajo(ApplicationDbContext db)
        {
            _db = db;
            Categoria = new CategoriaRepository(_db);
            Articulo = new ArticuloRepository(_db);
            Slider = new SliderRepository(_db);
            Usuario = new UsuarioRepository(_db);
        }

        public void Dispose() => _db.Dispose();

        public void Save() => _db.SaveChanges();
    }
}
