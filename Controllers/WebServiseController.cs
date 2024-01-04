using Microsoft.AspNetCore.Mvc;

namespace iakademi47_proje.Controllers
{
    public class WebServiseController : Controller
    {
        public static string tckimlikno = "";
        public static string vergino = "";
        public IActionResult Index()
        {
            return View();
        }
    }
}
