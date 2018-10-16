using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StreamWork.Models;

namespace StreamWork.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Math()
        {
            return View();
        }
        public IActionResult Literature()
        {
            return View();
        }
        public IActionResult Engineering()
        {
            return View();
        }
        public IActionResult DesignArt()
        {
            return View();
        }
        public IActionResult Science()
        {
            return View();
        }
        public IActionResult Business()
        {
            return View();
        }
        public IActionResult Programming()
        {
            return View();
        }
        public IActionResult Other()
        {
            return View();
        }

        public IActionResult SignUp()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }
       
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
