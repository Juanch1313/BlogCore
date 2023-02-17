using BlogCore.Data;
using BlogCore.Models;
using BlogCore.Utilidades;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogCore.AccesoDatos.Data.Inicializador
{
    public class InicializadorDb : IInizializadorDb
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public InicializadorDb(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Inicializar()
        {
            try
            {
                if(_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception)
            {

            }
            if (_db.Roles.Any(ro => ro.Name == CNT.Admin)) return;
            _roleManager.CreateAsync(new IdentityRole(CNT.Admin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(CNT.Usuario)).GetAwaiter().GetResult();

            _userManager.CreateAsync(new ApplicationUser{
                UserName = "juanchitorodi1312@gmail.com",
                Email = "juanchitorodi1312@gmail.com",
                EmailConfirmed = true,
                Nombre = "Juan"
            }, "Ju@nch1312").GetAwaiter().GetResult();

            ApplicationUser usuario = _db.ApplicationUsers
                .Where(us => us.Email == "juanchitorodi1312@gmail.com")
                .FirstOrDefault();
            
            _userManager.AddToRoleAsync(usuario, CNT.Admin).GetAwaiter().GetResult();
        }
    }
}
