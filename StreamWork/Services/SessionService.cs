using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StreamWork.DataModels;

namespace StreamWork.Services
{
    public class SessionService
    {
        private readonly IHttpContextAccessor httpContext;
        private readonly StorageService storage;

        public string Host { get; private set; }
        public bool Authenticated { get { return httpContext.HttpContext.User.Identity.IsAuthenticated; } }

        public SessionService (IHttpContextAccessor httpContext, StorageService storage)
        {
            this.httpContext = httpContext;
            this.storage = storage;
            Host = $"{httpContext.HttpContext.Request.Scheme}://{httpContext.HttpContext.Request.Host}";
        }

        public string Url(string path)
        {
            return Host + path;
        }

        public async Task<UserLogin> GetCurrentUser()
        {
            return await storage.Get<UserLogin>(HelperMethods.SQLQueries.GetUserWithUsername, new string[] { "rarunT" });
            //return await storage.GetUser(httpContext.HttpContext.User.Identity.Name);
        }
    }
}
