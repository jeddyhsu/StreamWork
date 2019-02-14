using Microsoft.AspNetCore.Mvc;

namespace StreamWork.Controllers
{
    public class HomePageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}