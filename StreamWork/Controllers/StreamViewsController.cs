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
        HelperFunctions helperFunctions = new HelperFunctions();

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> StreamPage([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string streamKeyandchatId, string tutor)
        {
            var user = HttpContext.Session.GetString("UserProfile");

            var split = streamKeyandchatId.Split(new char[] { '|' });
            var secretChatKey = helperFunctions.GetChatSecretKey(split[1], split[2], user);
            string[] arr = { split[0], secretChatKey };

            StreamPageViewModel model = new StreamPageViewModel {
                profile = new ProfileTutorViewModel {
                    userProfile = user != null ? await helperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, user) : null,
                    userProfile2 = await helperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, tutor)
                },
                urlParams = arr
            };
            return View ("StreamPage", model);
        }

        public async Task<IActionResult> StreamPlaybackPage([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string streamId)
        {
            var user = HttpContext.Session.GetString("UserProfile");
            string[] arr = { streamId };

            StreamPageViewModel model = new StreamPageViewModel {
                profile = new ProfileTutorViewModel {
                    userProfile = user != null ? await helperFunctions.GetUserProfile(storageConfig, QueryHeaders.CurrentUser, user) : null
                },
                urlParams = arr
            };
            return View("StreamPlaybackPage", model);
        }
    }
}