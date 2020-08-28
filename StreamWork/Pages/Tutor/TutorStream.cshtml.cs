﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRCoder;
using StreamWork.DataModels;
using StreamWork.HelperMethods;
using StreamWork.Services;

namespace StreamWork.Pages.Tutor
{
    public class TutorStream : PageModel
    {
        private readonly CookieService cookieService;
        private readonly StorageService storageService;
        private readonly StreamService streamService;
        private readonly NotificationService notificationService;
        private readonly EncryptionService encryptionService;
        private readonly ScheduleService scheduleService;

        public Profile CurrentUserProfile { get; set; }
        public Channel UserChannel { get; set; }
        public string ChatInfo { get; set; }
        public List<Notification> Notifications { get; set; }
        public bool AreThereUnseenNotifications { get; set; }
        public Schedule ScheduledStream { get; set; }
        public List<Schedule> Schedule { get; set; }
        public string ChatQrCode { get; set; }
        public string Host { get; set; }

        public TutorStream(StorageService storage, CookieService cookie, StreamService stream, NotificationService notification, EncryptionService encryption, ScheduleService schedule)
        {
            storageService = storage;
            cookieService = cookie;
            streamService = stream;
            notificationService = notification;
            encryptionService = encryption;
            scheduleService = schedule;
        }

        public async Task<IActionResult> OnGet(string scheduleId)
        {
            if (!cookieService.Authenticated || (await cookieService.GetCurrentUser()).ProfileType != "tutor")
            {
                return Redirect(cookieService.Url("/Home/SignIn/SW"));
            }

            CurrentUserProfile = await cookieService.GetCurrentUser();
            UserChannel = await storageService.Get<Channel>(SQLQueries.GetUserChannelWithUsername, new string[] { CurrentUserProfile.Username });
            ChatInfo = UserChannel.ArchivedVideoId != null ? encryptionService.EncryptString(UserChannel.ArchivedVideoId) : encryptionService.EncryptString("DEFAULT");

            Notifications = await notificationService.GetNotifications(CurrentUserProfile.Username);
            AreThereUnseenNotifications = await notificationService.AreThereUnseenNotifications(CurrentUserProfile.Username);

            ScheduledStream = await storageService.Get<Schedule>(SQLQueries.GetScheduleWithId, new string[] { scheduleId, DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd HH:mm") });
            Schedule = await scheduleService.GetSchedule(CurrentUserProfile.Username);

            GenerateQRCode(cookieService.Url("/Chat/Live/" + CurrentUserProfile.Username));
            Host = cookieService.host;

            return Page();
        }

        private void GenerateQRCode(string url)
        {
            using(MemoryStream ms = new MemoryStream())
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrData);
                using(Bitmap bitmap = qrCode.GetGraphic(20))
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    ChatQrCode = "data:image/png/;base64," + Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public IActionResult OnPostIsLive(string channelKey)
        {
            if (streamService.IsLive(channelKey)) return new JsonResult(new { Message = JsonResponse.Success.ToString() });
            return new JsonResult(new { Message = JsonResponse.Failed.ToString() });
        }

        public async Task<IActionResult> OnPostRegisterStream()
        {
            var userProfile = await cookieService.GetCurrentUser();
            var userChannel = await storageService.Get<Channel>(SQLQueries.GetUserChannelWithUsername, userProfile.Username);

            


            var archivedVideoId = streamService.StartStream(Request, userProfile, userChannel);
            if (archivedVideoId != null) return new JsonResult(new { Message = JsonResponse.Success.ToString(), Results = new string[] { userChannel.Username } }) ; //for chatbox
            return new JsonResult(new { Message = JsonResponse.Failed.ToString() });
        }
    }
}
