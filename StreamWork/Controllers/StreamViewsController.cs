using Microsoft.AspNetCore.Mvc;

namespace StreamWork.Controllers
{
    public class StreamViewsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult StreamPage(string streamKey) {

            return View ("StreamPage",streamKey);
        }
    }
}