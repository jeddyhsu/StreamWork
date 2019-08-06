using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.Core;
using StreamWork.Models;
using StreamWork.ViewModels;

namespace StreamWork.Controllers
{
    public class StudentController : Controller
    {
        private readonly string _connectionString = "Server=tcp:streamwork.database.windows.net,1433;Initial Catalog=StreamWork;Persist Security Info=False;User ID=streamwork;Password=arizonastate1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public async Task<IActionResult> ProfileStudent([FromServices] IOptionsSnapshot<StorageConfig> storageConfig)
        {
            var model = new UserProfile();
            var user = HttpContext.Session.GetString("UserProfile");
            var getUserInfo = await DataStore.GetListAsync<UserLogin>(_connectionString, storageConfig.Value, "PaticularSignedUpUsers", new List<string> { user });
            foreach (var u in getUserInfo)
            {
                var splitName = u.Name.Split(new char[] { '|' });
                model.FirstName = splitName[0];
                model.LastName = splitName[1];
            }

            ProfileStudentViewModel viewModel = new ProfileStudentViewModel
            {
                userProfile = model,
                userLogins = await DataStore.GetListAsync<UserLogin>(_connectionString, storageConfig.Value, "PaticularSignedUpUsers", new List<string> { user })
            };

            return View(viewModel);
        }
    }
}
