using Microsoft.AspNetCore.Mvc;

namespace StreamWork.Controllers
{
    public class StreamViewsController : Controller
    {
        public IActionResult Index( )
        {
            return View();
        }
        public IActionResult MathStream(string streamKey) {

            return View ("MathStream",streamKey);
        }
    }
}