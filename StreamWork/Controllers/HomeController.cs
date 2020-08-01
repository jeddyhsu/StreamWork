using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StreamWork.Config;
using Microsoft.Extensions.Options;
using StreamWork.HelperMethods;
using StreamWork.DataModels;
using System;
using StreamWork.Core;
using System.Collections.Generic;

namespace StreamWork.Controllers
{
    public class HomeController : Controller
    {

        public HomeMethods home = new HomeMethods();

        [HttpGet]
        public async Task<IActionResult> Index([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecordEmail([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string email)
        {
            Stealth s = new Stealth
            {
                Id = Guid.NewGuid().ToString(),
                Email = email
            };

            if(await DataStore.SaveAsync<Stealth>(home._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", s.Id } }, s))
            {
                return Json(new { Message = JsonResponse.Success.ToString() }); ;
            }

            return Json(new { Message = JsonResponse.Failed.ToString() }); ;

        }
    }
}
