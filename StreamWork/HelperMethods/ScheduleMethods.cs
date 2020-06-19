using System;
using System.Collections.Generic;
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

        public async Task<bool> AddToSchedule([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, HttpRequest request)
        {
            try
            {
                var name = request.Form["Name"];
                var username = request.Form["Username"];
                var streamTitle = request.Form["StreamTitle"];
                var streamSubject = request.Form["StreamSubject"];
                var timeStart = request.Form["TimeStart"];
                var timeStop = request.Form["TimeStop"];
                var timeZone = request.Form["TimeZone"];

                Schedule schedule = new Schedule
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = name,
                    Username = username,
                    StreamTitle = streamTitle,
                    StreamSubject = streamSubject,
                    TimeStart = timeStart,
                    TimeStop = timeStop,
                    TimeZone = timeZone
                };

                await DataStore.SaveAsync(_homeMethods._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", schedule.Id } }, schedule);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in ScheduleMethods:AddToSchedule " + e.Message);
                return false;
            }
        }

        public async Task<List<Schedule>> GetSchedule([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string user)
        {
            return await DataStore.GetListAsync<Schedule>(_homeMethods._connectionString, storageConfig.Value, SQLQueries.GetScheduleWithUserUsername.ToString(), new List<string> { user });
        }
    }
}
