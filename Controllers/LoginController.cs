using Microsoft.AspNetCore.Mvc;
using System.Security.Policy;
using TP4.Models;

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
        public IActionResult Logueo(int _dni, string _password)
        {

            try
            {
                Usuario usuario = _context.usuarios.Where(u => u._dni == _dni && !u._bloqueado && u._password == _password).FirstOrDefault();

                if (usuario != null)
                {
        
                    return Redirect("/Home/Index");
                }
          

            }
            catch (Exception ex) { }

            return Redirect("Index");
        }
    }
}
