using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamWork.DataModels;
using StreamWork.HelperMethods;
using StreamWork.Services;

namespace StreamWork.Pages.Home
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly StorageService storage;
        private readonly EmailService email;
        private readonly EncryptionService encryption;

        public ForgotPasswordModel(StorageService storage, EmailService email, EncryptionService encryption)
        {
            this.storage = storage;
            this.email = email;
            this.encryption = encryption;
        }

        private long RandomLong(long min, long max)
        {
            Random random = new Random();
            long result = random.Next((int)(min >> 32), (int)(max >> 32));
            result <<= 32;
            result |= (long)random.Next((int)min, (int)max);
            return result;
        }

        public async Task<JsonResult> OnGetSendChangePasswordEmail(string username)
        {
            // TODO Make a shorter way to get profile with either username or email
            Profile user = await storage.Get<Profile>(SQLQueries.GetUserWithUsername, username);
            if (user == null)
            {
                // The user can enter either username or email
                user = await storage.Get<Profile>(SQLQueries.GetUserWithEmailAddress, username);
            }

            // Should take about 9 years to brute force, if each test takes 0.06 sec
            user.ChangePasswordKey = RandomLong(1000000000, 9999999999).ToString();
            await storage.Save(user.Id, user);

            await email.SendTemplateToUser("changePassword", user, new List<System.IO.MemoryStream>());

            return new JsonResult(true);
        }

        public async Task<JsonResult> OnGetCheckChangePasswordCode(string username, string changePasswordCode)
        {
            Profile user = await storage.Get<Profile>(SQLQueries.GetUserWithUsername, username);
            if (user == null)
            {
                // The user can enter either username or email
                user = await storage.Get<Profile>(SQLQueries.GetUserWithEmailAddress, username);
            }

            return new JsonResult(user.ChangePasswordKey == changePasswordCode);
        }

        public async Task<JsonResult> OnGetChangePassword(string username, string changePasswordCode, string password)
        {
            Profile user = await storage.Get<Profile>(SQLQueries.GetUserWithUsername, username);
            if (user == null)
            {
                // The user can enter either username or email
                user = await storage.Get<Profile>(SQLQueries.GetUserWithEmailAddress, username);
            }

            if (user.ChangePasswordKey == changePasswordCode)
            {
                user.Password = encryption.EncryptPassword(password);
                await storage.Save(user.Id, user);
                return new JsonResult(true);
            }

            return new JsonResult(false);
        }
    }
}
