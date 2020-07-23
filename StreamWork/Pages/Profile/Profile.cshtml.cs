using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.HelperMethods;
using StreamWork.Services;

namespace StreamWork.Pages.Profile
{
    public class Profile : PageModel
    {
        private readonly SessionService sessionService;
        private readonly ProfileService profileService;
        private readonly EditService editService;

        public Profile(StorageService storage, SessionService session, ProfileService profile, ScheduleService schedule, FollowService follow, EditService edit, SearchService search)
        {
            sessionService = session;
            profileService = profile;
            editService = edit;
        }

        public async Task<IActionResult> OnPostSaveProfile()
        {
            var userProfile = await sessionService.GetCurrentUser();
            var savedInfo = await editService.EditProfile(Request, userProfile.Username);

            if (savedInfo != null) return new JsonResult(new { Message = JsonResponse.Success.ToString(), SavedInfo = savedInfo });
            return new JsonResult(new { Message = JsonResponse.Failed.ToString() });
        }

        public async Task<IActionResult> OnPostSaveSection()
        {
            var userProfile = await sessionService.GetCurrentUser();

            if (profileService.SaveSection(Request, userProfile)) return new JsonResult(new { Message = JsonResponse.Success.ToString() });
            return new JsonResult(new { Message = JsonResponse.Failed.ToString() });
        }

        public async Task<IActionResult> OnPostSaveTopic()
        {
            var userProfile = await sessionService.GetCurrentUser();

            if (profileService.SaveTopic(Request, userProfile)) return new JsonResult(new { Message = JsonResponse.Success.ToString() });
            return new JsonResult(new { Message = JsonResponse.Failed.ToString() });
        }

        public async Task<IActionResult> OnPostSaveBanner()
        {
            var userProfile = await sessionService.GetCurrentUser();
            var banner = await editService.SaveBanner(Request, userProfile.Username);

            if (banner != null) return new JsonResult(new { Message = JsonResponse.Success.ToString(), Banner = banner });
            return new JsonResult(new { Message = JsonResponse.Success.ToString(), Banner = banner });
        }

        public async Task<IActionResult> OnPostSaveUniversity(string abbr, string name)
        {
            var userProfile = await sessionService.GetCurrentUser();

            if (await editService.SaveUniversity(userProfile.Username, abbr, name)) return new JsonResult(new { Message = JsonResponse.Success.ToString(), SavedInfo = new List<string> { abbr, name } });
            return new JsonResult(new { Message = JsonResponse.Failed.ToString() });
        }
    }
}
