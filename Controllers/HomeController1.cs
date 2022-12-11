using Microsoft.AspNetCore.Mvc;

namespace TP4.Controllers
{
    public class HomeController1 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
