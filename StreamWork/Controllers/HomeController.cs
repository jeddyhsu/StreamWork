using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StreamWork.Models;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using StreamWork.Base;
using StreamWork.Core;
using StreamWork.Config;
using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace StreamWork.Controllers
{
    public class HomeController : Controller
    {

        private readonly string _connectionString = "Data Source=localhost;Initial Catalog=Streamwork;Integrated Security=false;User=SA;Password=Streamwork2018";

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

        [HttpPost]      
        public async Task<IActionResult> SignUp( [FromServices] IOptionsSnapshot<StorageConfig> storageConfig, 
                                                string nameFirst, string nameLast, string email, string phone, string username, string password, string passwordConfirm)
        {
            string confirmation = "";
            StreamWorkSignUp signUpProflie = new StreamWorkSignUp
            {
               Id = Guid.NewGuid().ToString(),
                Name = nameFirst + "|" + nameLast,
                EmailAddress = email,
                PhoneNumber = phone,
                Username = username,
                Password = password,
            };
            var checkCurrentUsers = await DataStore.GetListAsync<StreamWorkSignUp>(_connectionString, storageConfig.Value, "AllSignedUpUsers", new List<string> { username });
            int numberOfUsers = 0;
            foreach(var user in checkCurrentUsers)
            {
                numberOfUsers++;
            }
            if (numberOfUsers != 0)
                confirmation = "Username already exsists";
            else if (password != passwordConfirm)
                confirmation = "Wrong Password";
            else
            {
                var success = await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", signUpProflie.Id } }, signUpProflie);
                confirmation = "Success";
            }
            return Json(new { Message = confirmation });
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromServices] IOptionsSnapshot<StorageConfig> storageConfig , string username, string password)
        {
            string confirmation = "";
            if (storageConfig != null)
            {
                int user = 0;
                var checkforUser = await DataStore.GetListAsync<StreamWorkSignUp>(_connectionString, storageConfig.Value, "AllSignedUpUsers", new List<string> { username, password });
                foreach (var u in checkforUser)
                {
                    user++;
                }
                if (user ==1)
                {
                    confirmation = "Welcome";
                    HttpContext.Session.SetString("UserProfile", username);
                }             
            }
            else
            {
                confirmation = "Wrong Password or Username ";
            }

            return Json(new { Message = confirmation });
        }

        [HttpGet]
        public IActionResult Login()
        {     
            return View();
        }

        public async Task<IActionResult> Profile([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            var model = new UserProfile();
            var user = HttpContext.Session.GetString("UserProfile");
            var getUserInfo = await DataStore.GetListAsync<StreamWorkSignUp>(_connectionString, storageConfig.Value, "GetUserInfo", new List<string> { user});
            foreach (var u in getUserInfo)
            {
                var splitName = u.Name.Split(new char[] { '|' });
                model.FirstName = splitName[0];
                model.LastName = splitName[1];
            }
            return View(model);
        }
       
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
