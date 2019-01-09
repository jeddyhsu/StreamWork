using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace StreamWork.Controllers
{
    public class StreamViewsController : Controller
    {
        public IActionResult Index( )
        {
            return View();
        }
        public IActionResult MathStream()
        {
            return View();
        }

    }
}