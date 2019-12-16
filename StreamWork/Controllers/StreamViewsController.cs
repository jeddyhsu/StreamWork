using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StreamWork.ViewModels;
using StreamWork.HelperClasses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using StreamWork.Config;

namespace StreamWork.Controllers
{
    public class StreamViewsController : Controller
    {
        HomeHelperFunctions _homehelperFunctions = new HomeHelperFunctions();

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> StreamPage([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string streamKeyandchatId, string tutor)
        {
            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homehelperFunctions._host + "/Home/Login?dest=-StreamViews-StreamPage");

            var split = streamKeyandchatId.Split(new char[] { '|' });
            var secretChatKey = _homehelperFunctions.GetChatSecretKey(split[1], split[2], User.Identity.Name);
            string[] arr = { split[0], secretChatKey };

            StreamPageViewModel model = new StreamPageViewModel {
                profile = new ProfileTutorViewModel {
                    userProfile = User.Identity.Name != null ? await _homehelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, User.Identity.Name) : null,
                    studentOrtutorProfile = await _homehelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, tutor)
                },
                urlParams = arr
            };

            return View ("StreamPage", model);
        }

        public async Task<IActionResult> StreamPlaybackPage([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string streamId)
        {
            if (HttpContext.User.Identity.IsAuthenticated == false)
                return Redirect(_homehelperFunctions._host + "/Home/Login?dest=-StreamViews-StreamPlaybackPage");

            string[] arr = { streamId };
            StreamPageViewModel model = new StreamPageViewModel {
                profile = new ProfileTutorViewModel {
                    userProfile = User.Identity.Name != null ? await _homehelperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, User.Identity.Name) : null
                },
                urlParams = arr
            };

            return View("StreamPlaybackPage", model);
        }
    }
}