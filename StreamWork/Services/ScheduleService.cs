using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.DataModels;
using StreamWork.HelperMethods;

namespace StreamWork.Services
{
    public class ScheduleService : StorageService
    {
        public ScheduleService([FromServices] IOptionsSnapshot<StorageConfig> config) : base(config) { }

        public async Task<List<Schedule>> SaveToSchedule(HttpRequest request, string user)
        {
            try
            {
                Schedule schedule = null;
                var userProfile = await Get<UserLogin>(SQLQueries.GetUserWithUsername, new string[] { user });

                var id = request.Form["Id"];
                var streamTitle = request.Form["StreamTitle"];
                var streamSubject = request.Form["StreamSubject"];
                var timeStop = request.Form["TimeStop"];
                var timeZone = userProfile.TimeZone;

                var date = DateTime.ParseExact(request.Form["Date"], "ddd MMM dd yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                var timeStart = DateTime.ParseExact(request.Form["TimeStart"], "h:mm tt", CultureInfo.InvariantCulture);

                DateTime newDate = new DateTime(date.Year, date.Month, date.Day, timeStart.Hour, timeStart.Minute, timeStart.Second);

                if (id.Equals("undefined"))
                {
                    schedule = new Schedule
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = userProfile.Name,
                        Username = userProfile.Username,
                        StreamTitle = streamTitle,
                        StreamSubject = streamSubject,
                        TimeStart = timeStart.ToString("h:mm tt"),
                        TimeStop = timeStop,
                        TimeZone = timeZone,
                        Date = newDate,
                        SubjectThumbnail = MiscHelperMethods.GetCorrespondingSubjectThumbnail(streamSubject)
                    };
                }
                else
                {
                    schedule = (await GetList<Schedule>(SQLQueries.GetScheduleWithId, new string[] { id, DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd HH:mm") }))[0];
                    schedule.StreamTitle = streamTitle;
                    schedule.StreamSubject = streamSubject;
                    schedule.TimeStart = timeStart.ToString("h:mm tt");
                    schedule.TimeStop = timeStop;
                    schedule.TimeZone = timeZone;
                    schedule.Date = newDate;
                    schedule.SubjectThumbnail = MiscHelperMethods.GetCorrespondingSubjectThumbnail(streamSubject);
                }

                await Save<Schedule>(schedule.Id, schedule);
                return await GetSchedule(user);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in ScheduleMethods:AddToSchedule " + e.Message);
                return null;
            }
        }

        public async Task<List<Schedule>> GetSchedule(string user)
        {
            await Run<Schedule>(SQLQueries.DeletePastScheduledTasks, new string[] { DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd HH:mm") });
            return await GetList<Schedule>( SQLQueries.GetScheduleWithUserUsername, new string[] { user, DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd HH:mm") });
        }

        public async Task<List<Schedule>> DeleteFromSchedule(string id, string user)
        {
            var saved = await Run<Schedule>(SQLQueries.DeleteScheduleTaskWithId, new string[] { id });
            if (saved) return await GetSchedule(user);
            return null;
        }

        public async Task<bool> UpdateTimezoneForScheduleTask(string timezone, string username)
        {
            return await Run<Schedule>(SQLQueries.UpdateTimezonesOfScheduledTasks, new string[]{ timezone, username });
        }
    }
}
