using Microsoft.AspNetCore.Mvc;

namespace StreamWork.Controllers
{
    public class StreamViewsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult StreamPage(string streamKeyandchatId)
        {
            var split = streamKeyandchatId.Split(new char[] { '|' });
            string[] arr = { split[0], split[1], split[2] };
            return View ("StreamPage", arr);
        }

        public IActionResult StreamPlaybackPage(string streamId)
        {
            return View("StreamPlaybackPage", streamId);
        }
    }
}