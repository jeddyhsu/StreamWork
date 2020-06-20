using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.Core;
using StreamWork.DataModels;

namespace StreamWork.HelperMethods
{
    public class ScheduleMethods
    {
        private readonly HomeMethods _homeMethods = new HomeMethods();
        private readonly TutorMethods _tutorMethods = new TutorMethods();

        public async Task<List<Schedule>> SaveToSchedule([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, HttpRequest request, string user)
        {
            try
            {
                var userProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, user);

                var streamTitle = request.Form["StreamTitle"];
                var streamSubject = request.Form["StreamSubject"];
                var timeZone = request.Form["TimeZone"];

                var format = "ddd MMM dd yyyy HH:mm:ss";
                var timeStart = DateTime.ParseExact(request.Form["TimeStart"], format, CultureInfo.InvariantCulture);
                var timeStop = DateTime.ParseExact(request.Form["TimeStop"], format, CultureInfo.InvariantCulture);
                var date = DateTime.ParseExact(request.Form["Date"], format, CultureInfo.InvariantCulture);

                DateTime newDate = new DateTime(date.Year, date.Month, date.Day, timeStart.Hour, timeStart.Minute, timeStart.Second);

                Schedule schedule = new Schedule
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = userProfile.Name,
                    Username = userProfile.Username,
                    StreamTitle = streamTitle,
                    StreamSubject = streamSubject,
                    TimeStart = timeStart.ToString("h:mm tt"),
                    TimeStop = timeStop.ToString("h:mm tt"),
                    TimeZone = timeZone,
                    Date = newDate,
                    SubjectThumbnail = _homeMethods.GetCorrespondingSubjectThumbnail(streamSubject)
                };

                await DataStore.SaveAsync(_homeMethods._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", schedule.Id } }, schedule);
                return await GetSchedule(storageConfig, user);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in ScheduleMethods:AddToSchedule " + e.Message);
                return null;
            }
        }

        public async Task<List<Schedule>> GetSchedule([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string user)
        {
            return await DataStore.GetListAsync<Schedule>(_homeMethods._connectionString, storageConfig.Value, SQLQueries.GetScheduleWithUserUsername.ToString(), new List<string> { user });
        }
    }
}
