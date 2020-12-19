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
using StreamWork.DataModels;
using StreamWork.HelperMethods;

namespace StreamWork.Services
{
    public class CookieService : StorageService
    {
        public static bool devEnvironment;
        public readonly string host = devEnvironment ? "http://localhost:58539" : "https://streamwork.live";
        private readonly IHttpContextAccessor httpContext;
        public bool Authenticated;

        public CookieService([FromServices] IOptionsSnapshot<StorageConfig> config, IHttpContextAccessor httpContext) : base(config)
        {
            this.httpContext = httpContext;
            Authenticated = httpContext.HttpContext.User.Identity.IsAuthenticated;
        }

        public async Task<Profile> SignIn(string username, string password)
        {

            var userProfile = await Get<Profile>(SQLQueries.GetUserWithUsernameAndPassword, new string[] { username, password });

            if (userProfile != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userProfile.Username),
                    new Claim(ClaimTypes.Email, userProfile.EmailAddress),
                    new Claim(ClaimTypes.NameIdentifier, userProfile.Username)
                    //new Claim(ClaimTypes.UserData, MiscHelperMethods.GetRandomChatColor())
                };

                var userIdentity = new ClaimsIdentity(claims, "cookie");

                ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
                await httpContext.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                userProfile.LastLogin = DateTime.UtcNow;
                await Save(userProfile.Id, userProfile);

                return userProfile;
            }

            return null;
        }

        public async Task SignOut()
        {
            await httpContext.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public async Task<Profiles> ValidateUser(string username)
        {
            var userProfile = await Get<Profiles>(SQLQueries.GetUserWithUsername, new string[] { username });
            return userProfile;
        }

        public string Url(string path)
        {
            return host + path;
        }

        public async Task<Profiles> GetCurrentUser()
        {
            if (httpContext.HttpContext.User.Identity.Name == null) return null;

            return await Get<Profiles>(SQLQueries.GetUserWithUsername, new string[] { httpContext.HttpContext.User.Identity.Name });
        }

        public async Task<bool> ValidateUserType(string username, string type)
        {
            var currentUser = await Get<Profiles>(SQLQueries.GetUserWithUsername, username);
            if (currentUser.ProfileType == type) return true;

            return false;
        }

        public JsonResult Route(Profiles userProfile, string route, EncryptionService encryptionService)
        {
            if (userProfile != null)
            {
                if (route != "SW")
                {
                    var decryptedRoute = encryptionService.DecryptString(route);
                    return new JsonResult(new { Message = "Route", Route = decryptedRoute });
                }
                else if (userProfile.ProfileType == "tutor")
                    return new JsonResult(new { Message = JsonResponse.Tutor.ToString() });
                else
                    return new JsonResult(new { Message = JsonResponse.Student.ToString() });
            }
            else
            {
                return new JsonResult(new { Message = JsonResponse.Failed.ToString() });
            }
        }
    }
}
