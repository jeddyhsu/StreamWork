using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StreamWork.Models;
using Microsoft.AspNetCore.Http;
using StreamWork.Core;
using StreamWork.Config;
using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace StreamWork.Controllers
{
    public class HomeController : Controller
    {

        private readonly string _connectionStringYoutube = "Server=tcp:streamwork.database.windows.net,1433;Initial Catalog=YouTubeStreamKeys;Persist Security Info=False;User ID=streamwork;Password=arizonastate1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        private readonly string _connectionStringSignIn = "Server=tcp:streamwork.database.windows.net,1433;Initial Catalog=StreamWorkSignUp;Persist Security Info=False;User ID=streamwork;Password=arizonastate1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";


        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Math([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            var getAllStreams = await DataStore.GetListAsync<YoutubeStreamKeys>(_connectionStringYoutube, storageConfig.Value, "AllStreamKeys", new List<string> { });
            return View(getAllStreams);
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
        public async Task<IActionResult> SignUp([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, 
                                                string nameFirst, string nameLast, string email, string phone, string username, string password, string passwordConfirm, string role)
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
                ProfileType = role
            };
            var checkCurrentUsers = await DataStore.GetListAsync<StreamWorkSignUp>(_connectionStringSignIn, storageConfig.Value, "AllSignedUpUsers", new List<string> { username });
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
                var success = await DataStore.SaveAsync(_connectionStringSignIn, storageConfig.Value, new Dictionary<string, object> { { "Id", signUpProflie.Id } }, signUpProflie);
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
                var checkforUser = await DataStore.GetListAsync<StreamWorkSignUp>(_connectionStringSignIn, storageConfig.Value, "AllSignedUpUsers1", new List<string> { username, password });
                foreach (var u in checkforUser)
                {
                    if(u.Password == password && u.Username == username)
                    {
                        user++;
                    }
                }
                if (user == 1)
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

        public async Task<IActionResult> ProfileStudent([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            var model = new UserProfile();
            var user = HttpContext.Session.GetString("UserProfile");
            var getUserInfo = await DataStore.GetListAsync<StreamWorkSignUp>(_connectionStringSignIn, storageConfig.Value, "GetUserInfo", new List<string> {user});
            foreach (var u in getUserInfo)
            {
                var splitName = u.Name.Split(new char[] { '|' });
                model.FirstName = splitName[0];
                model.LastName = splitName[1];
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ProfileTutor([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            var model = new UserProfile();
            var user = HttpContext.Session.GetString("UserProfile");
            var getUserInfo = await DataStore.GetListAsync<StreamWorkSignUp>(_connectionStringSignIn, storageConfig.Value, "GetUserInfo", new List<string> { user });
            foreach (var u in getUserInfo)
            {
                var splitName = u.Name.Split(new char[] { '|' });
                model.FirstName = splitName[0];
                model.LastName = splitName[1];
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ProfileTutor([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string streamKey)
        {
            YoutubeStreamKeys key = new YoutubeStreamKeys
            {
                Id = Guid.NewGuid().ToString(),
                Username = HttpContext.Session.GetString("UserProfile"),
                StreamKey = streamKey
            };
            var success = await DataStore.SaveAsync(_connectionStringYoutube, storageConfig.Value, new Dictionary<string, object> { { "Id", key.Id } }, key);
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
