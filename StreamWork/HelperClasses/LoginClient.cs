using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.Core;
using StreamWork.DataModels;
using StreamWork.HelperMethods;

namespace StreamWork.HelperClasses
{
    public class LoginClient
    {
        private readonly HomeMethods _homeMethods = new HomeMethods();
        private readonly EncryptionMethods _encryptionMethods = new EncryptionMethods();
        private readonly IOptionsSnapshot<StorageConfig> _storageConfig;
        private readonly HttpContext _context;
        private readonly string _username;
        private readonly string _password;

        public LoginClient([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, HttpContext context, string username, string password)
        {
            _storageConfig = storageConfig;
            _context = context;
            _username = username;
            _password = password;
        }

        public async Task<string> Login()
        {
            var userProfile = await _homeMethods.GetUserProfile(_storageConfig, SQLQueries.GetUserWithUsername, _username);
            if (userProfile == null) return JsonResponse.WrongUsernameOrPassword.ToString();
            var checkforUser = await DataStore.GetListAsync<UserLogin>(_homeMethods._connectionString, _storageConfig.Value, SQLQueries.GetUserWithUsernameAndPassword.ToString(), new List<string> { _username, _encryptionMethods.DecryptPassword(userProfile.Password, _password) });
            // Uncomment the following line and comment the above line to login as any user. Remember to reverse this before publishing!
            // var checkforUser = new List<UserLogin> { userProfile };
            if (checkforUser.Count == 1)
            {
                _context.Session.SetString(SQLQueries.UserProfile.ToString(), _username);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, _username),
                    new Claim(ClaimTypes.Email, userProfile.EmailAddress),
                    new Claim(ClaimTypes.UserData, _homeMethods.GetRandomChatColor())
                };

                var userIdentity = new ClaimsIdentity(claims, "cookie");

                ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
                await _context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                userProfile.LastLogin = DateTime.UtcNow;
                await _homeMethods.UpdateUser(_storageConfig, userProfile);

                if (userProfile.ProfileType == "tutor") return JsonResponse.Tutor.ToString();
                else return JsonResponse.Student.ToString();
            }

            return JsonResponse.WrongUsernameOrPassword.ToString();
        }
    }
}
