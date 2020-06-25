using System;
using System.Collections;
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

        public async Task<List<Schedule>> SaveToSchedule([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, HttpRequest request, string user)
        {
            try
            {
                Schedule schedule = null;
                var userProfile = await _homeMethods.GetUserProfile(storageConfig, SQLQueries.GetUserWithUsername, user);

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
                        SubjectThumbnail = _homeMethods.GetCorrespondingSubjectThumbnail(streamSubject)
                    };
                }
                else
                {
                    schedule = (await DataStore.GetListAsync<Schedule>(_homeMethods._connectionString, storageConfig.Value, SQLQueries.GetScheduleWithId.ToString(), new List<string> { id, DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd HH:mm") }))[0];
                    schedule.StreamTitle = streamTitle;
                    schedule.StreamSubject = streamSubject;
                    schedule.TimeStart = timeStart.ToString("h:mm tt");
                    schedule.TimeStop = timeStop;
                    schedule.TimeZone = timeZone;
                    schedule.Date = newDate;
                    schedule.SubjectThumbnail = _homeMethods.GetCorrespondingSubjectThumbnail(streamSubject);
                }

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
            await DataStore.RunQueryAsync<Schedule>(_homeMethods._connectionString, storageConfig.Value, SQLQueries.DeletePastScheduledTasks.ToString(), new List<string> { DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd HH:mm") });
            return await DataStore.GetListAsync<Schedule>(_homeMethods._connectionString, storageConfig.Value, SQLQueries.GetScheduleWithUserUsername.ToString(), new List<string> { user, DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd HH:mm") });
        }

        public async Task<List<Schedule>> DeleteFromSchedule([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string id, string user)
        {
            var saved = await DataStore.RunQueryAsync<Schedule>(_homeMethods._connectionString, storageConfig.Value, SQLQueries.DeleteScheduleTaskWithId.ToString(), new List<string> { id });
            if (saved) return await GetSchedule(storageConfig, user);
            return null;
        }

        public async Task<bool> UpdateTimezoneForScheduleTask([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string timezone, string username)
        {
            return await DataStore.RunQueryAsync<Schedule>(_homeMethods._connectionString, storageConfig.Value, SQLQueries.UpdateTimezonesOfScheduledTasks.ToString(), new List<string> {timezone, username});
        }
    }
}
