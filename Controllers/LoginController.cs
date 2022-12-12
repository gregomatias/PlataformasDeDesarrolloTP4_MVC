using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Policy;
using TP4.Models;

//1.- REFERENCES AUTHENTICATION COOKIE
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;


namespace TP4.Controllers
{
    public class LoginController : Controller
    {
        private readonly MyContext _context;

        public LoginController(MyContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logueo(int _dni, string _password)
        {

            try
            {
                Usuario usuario = _context.usuarios.Where(u => u._dni == _dni && !u._bloqueado && u._password == _password).FirstOrDefault();

                if (usuario != null)
                {

                    //2.- CONFIGURACION DE LA AUTENTICACION
                    #region AUTENTICACTION
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario._apellido),
                    new Claim("Correo", usuario._mail),
                    new Claim("EsAdmin", usuario._esUsuarioAdmin.ToString()),
                     new Claim(ClaimTypes.Role, usuario._esUsuarioAdmin.ToString()),
                     
                };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                    #endregion


                 //   return Redirect("/Home/Index");
                 //   return RedirectToAction("Index", "Home");
                    return RedirectToAction("Index", "Home", new { IdUsuarioLogueado = usuario._id_usuario });
                }


            }
            catch (Exception ex) { }

            return Redirect("Index");
        }

        public async Task<IActionResult> Salir()
        {
            //3.- CONFIGURACION DE LA AUTENTICACION
            #region AUTENTICACTION
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            #endregion

            return RedirectToAction("Index");

        }

    }
}
