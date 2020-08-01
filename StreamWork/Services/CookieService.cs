using System;
using System.Collections.Generic;
using System.Linq;
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
        public readonly string host = devEnvironment ? "http://localhost:58539" : "https://www.streamwork.live";
        private readonly IHttpContextAccessor httpContext;
        public bool Authenticated;

        public CookieService([FromServices] IOptionsSnapshot<StorageConfig> config, IHttpContextAccessor httpContext) : base(config)
        {
            this.httpContext = httpContext;
            Authenticated = httpContext.HttpContext.User.Identity.IsAuthenticated;
        }

        public async Task<UserLogin> SignIn(string username, string password)
        {

            var userProfile = await Get<UserLogin>(SQLQueries.GetUserWithUsernameAndPassword, new string[] { username, password });

            if (userProfile != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userProfile.Username),
                    new Claim(ClaimTypes.Email, userProfile.EmailAddress),
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

        public async Task<UserLogin> ValidateUser(string username)
        {
            var userProfile = await Get<UserLogin>(SQLQueries.GetUserWithUsername, new string[] { username });
            return userProfile;
        }

        public string Url(string path)
        {
            return host + path;
        }

        public async Task<UserLogin> GetCurrentUser()
        {
            if (httpContext.HttpContext.User.Identity.Name == null) return null;

            return await Get<UserLogin>(SQLQueries.GetUserWithUsername, new string[] { httpContext.HttpContext.User.Identity.Name });
        }

        public async Task<bool> ValidateUserType(string username, string type)
        {
            var currentUser = await Get<UserLogin>(SQLQueries.GetUserWithUsername, username);
            if (currentUser.ProfileType == type) return true;

            return false;
        }
    }
}
