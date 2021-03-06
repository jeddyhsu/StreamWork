﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.DataModels;
using StreamWork.HelperMethods;
using StreamWork.Services;

namespace StreamWork.Pages.Profiles
{
    public class ProfileModal : PageModel
    {
        private readonly StorageService storageService;
        private readonly CookieService cookieService;
        private readonly ProfileService profileService;
        private readonly EditService editService;

        public ProfileModal(StorageService storage, CookieService cookie, ProfileService profile, EditService edit)
        {
            storageService = storage;
            cookieService = cookie;
            profileService = profile;
            editService = edit;
        }

        public async Task<IActionResult> OnPostSaveProfile()
        {
            var userProfile = await cookieService.GetCurrentUser();
            var savedInfo = await editService.EditProfile(Request, userProfile);

            if (savedInfo != null) return new JsonResult(new { Message = JsonResponse.Success.ToString(), SavedInfo = savedInfo });
            return new JsonResult(new { Message = JsonResponse.Failed.ToString() });
        }

        public async Task<IActionResult> OnPostDeleteProfilePicture()
        {
            var userProfile = await cookieService.GetCurrentUser();
            var savedInfo = await editService.DeleteProfilePicture(userProfile);

            if (savedInfo != null) return new JsonResult(new { Message = JsonResponse.Success.ToString(), Image = savedInfo });
            return new JsonResult(new { Message = JsonResponse.Failed.ToString() });
        }

        public async Task<IActionResult> OnPostSaveSection()
        {
            var userProfile = await cookieService.GetCurrentUser();

            if (profileService.SaveSection(Request, userProfile)) return new JsonResult(new { Message = JsonResponse.Success.ToString() });
            return new JsonResult(new { Message = JsonResponse.Failed.ToString() });
        }

        public async Task<IActionResult> OnPostSaveTopic()
        {
            var userProfile = await cookieService.GetCurrentUser();

            if (profileService.SaveTopic(Request, userProfile)) return new JsonResult(new { Message = JsonResponse.Success.ToString() });
            return new JsonResult(new { Message = JsonResponse.Failed.ToString() });
        }

        public async Task<IActionResult> OnPostSaveBanner()
        {
            var userProfile = await cookieService.GetCurrentUser();
            var banner = await editService.SaveBanner(Request, userProfile);

            if (banner != null) return new JsonResult(new { Message = JsonResponse.Success.ToString(), Banner = banner });
            return new JsonResult(new { Message = JsonResponse.Success.ToString(), Banner = banner });
        }

        public async Task<IActionResult> OnPostSaveUniversity(string abbr, string name)
        {
            var userProfile = await cookieService.GetCurrentUser();

            if (await editService.SaveUniversity(userProfile, abbr, name)) return new JsonResult(new { Message = JsonResponse.Success.ToString(), SavedInfo = new List<string> { abbr, name } });
            return new JsonResult(new { Message = JsonResponse.Failed.ToString() });
        }

        public async Task<IActionResult> OnPostChangeColor(string color)
        {
            var userProfile = await cookieService.GetCurrentUser();

            if (await profileService.ChangeColor(userProfile, color)) return new JsonResult(new { Message = JsonResponse.Success.ToString() });
            return new JsonResult(new { Message = JsonResponse.Failed.ToString() });
        }

        public async Task<IActionResult> OnPostSearchVideos(string username)
        {
            return new JsonResult(new { Message = JsonResponse.Success.ToString(), Videos = await storageService.GetList<Video>(SQLQueries.GetArchivedStreamsWithUsername, username) });
        }
    }
}
