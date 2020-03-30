using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StreamWork.Config;
using StreamWork.Core;
using StreamWork.DataModels;
using StreamWork.TutorObjects;

namespace StreamWork.HelperClasses
{
    public class TutorHelperFunctions //For functions involved with tutor code only
    {
        readonly HomeHelperFunctions _homeHelperFunctions = new HomeHelperFunctions();

        //Uses a hashtable to add default thumbnails based on subject
        public string GetCorrespondingDefaultThumbnail(string subject)
        {
            string defaultURL = "";

            Hashtable defaultPic = new Hashtable
            {
                { "Mathematics", "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/MathDefault.png" },
                { "Humanities", "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/HumanitiesDefault.png" },
                { "Science", "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/ScienceDefault.png" },
                { "Business", "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/BusinessDefault.png" },
                { "Engineering", "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/EngineeringDefault.png" },
                { "Law", "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/LawDefault.png" },
                { "Art", "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/ArtDefault.png" },
                { "Other", "https://streamworkblob.blob.core.windows.net/streamworkblobcontainer/OtherDefualt.png" }
            };

            ICollection key = defaultPic.Keys;

            foreach (string pic in key)
            {
                if (pic == subject)
                {
                    defaultURL = ((string)defaultPic[pic]);
                }
            }

            return defaultURL;
        }

        public async Task ChangeAllArchivedStreamAndUserChannelProfilePhotos([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string user, string profilePicture) //changes all profile photos on streams if user has changed it
        {
            var allArchivedStreams = await _homeHelperFunctions.GetArchivedStreams(storageConfig, QueryHeaders.UserArchivedVideos, user);
            var userChannel = await _homeHelperFunctions.GetUserChannels(storageConfig, QueryHeaders.CurrentUserChannel, user);
            foreach (var stream in allArchivedStreams)
            {
                stream.ProfilePicture = profilePicture;
                await DataStore.SaveAsync(_homeHelperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", stream.Id } }, stream);
            }
            userChannel[0].ProfilePicture = profilePicture;
            await DataStore.SaveAsync(_homeHelperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userChannel[0].Id } }, userChannel[0]);
        }

        public List<Day> GetTutorStreamSchedule(UserChannel channel)
        {
            var todaysDate = DateTime.UtcNow.AddHours(GetHoursAheadBasedOnTimeZone() - (GetHoursAheadBasedOnTimeZone() * 2));

            Schedule schedule = new Schedule();

            schedule.Days.Add(todaysDate.ToString("MM/dd/yyyy"), new Day(todaysDate));
            schedule.Days.Add(todaysDate.AddDays(1.0).ToString("MM/dd/yyyy"), new Day(todaysDate.AddDays(1.0)));
            schedule.Days.Add(todaysDate.AddDays(2.0).ToString("MM/dd/yyyy"), new Day(todaysDate.AddDays(2.0)));
            schedule.Days.Add(todaysDate.AddDays(3.0).ToString("MM/dd/yyyy"), new Day(todaysDate.AddDays(3.0)));
            schedule.Days.Add(todaysDate.AddDays(4.0).ToString("MM/dd/yyyy"), new Day(todaysDate.AddDays(4.0)));
            schedule.Days.Add(todaysDate.AddDays(5.0).ToString("MM/dd/yyyy"), new Day(todaysDate.AddDays(5.0)));
            schedule.Days.Add(todaysDate.AddDays(6.0).ToString("MM/dd/yyyy"), new Day(todaysDate.AddDays(6.0)));
            schedule.Days.Add(todaysDate.AddDays(7.0).ToString("MM/dd/yyyy"), new Day(todaysDate.AddDays(7.0)));

            FormatTutorStreamSchedule(schedule, channel);

            ICollection collection = schedule.Days.Keys;
            List<Day> days = new List<Day>();

            foreach (string key in collection)
                days.Add(schedule.Days[key]);

            return days;
        }

        private void FormatTutorStreamSchedule(Schedule schedule, UserChannel channel)
        {
            if (channel.StreamTasks == null)
            {
                return;
            }

            var streamTaskList = JsonConvert.DeserializeObject<List<StreamTask>>(channel.StreamTasks);

            foreach (var streamTask in streamTaskList)
            {
                if (schedule.Days.ContainsKey(streamTask.Day))
                {
                    var day = schedule.Days[streamTask.Day];
                    day.StreamTaskList.Add(streamTask);
                }
                else
                {
                    streamTaskList.ToList().Remove(streamTask);
                }
            }
        }

        public async Task<bool> AddStreamTask([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string streamName, DateTime dateTime, UserChannel channel)
        {
            try
            {
                if (channel.StreamTasks != null)
                {
                    var streamTasksList = JsonConvert.DeserializeObject<List<StreamTask>>(channel.StreamTasks);
                    
                    streamTasksList.Add(new StreamTask(streamName, dateTime.ToString("h:mm tt"), dateTime.ToString("MM/dd/yyyy")));

                    streamTasksList = SortStreamTasksList(streamTasksList);

                    var serialize = JsonConvert.SerializeObject(streamTasksList);
                    channel.StreamTasks = serialize;
                    await DataStore.SaveAsync(_homeHelperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", channel.Id } }, channel);

                    return true;
                }
                else
                {
                    List<StreamTask> streamTasks = new List<StreamTask>();

                    streamTasks.Add(new StreamTask(streamName, dateTime.ToString("h:mm tt"), dateTime.ToString("MM/dd/yyyy")));

                    var serialize = JsonConvert.SerializeObject(streamTasks);
                    channel.StreamTasks = serialize;
                    await DataStore.SaveAsync(_homeHelperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", channel.Id } }, channel);

                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in AddStreamTask: " + e.Message);
                return false;
            }
        }

        public async Task<bool> UpdateStreamTask([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string streamName, DateTime dateTime, DateTime originalDateTime, UserChannel channel)
        {
            try
            {
                var streamTasksList = JsonConvert.DeserializeObject<List<StreamTask>>(channel.StreamTasks);

                foreach (var task in streamTasksList)
                {
                    if (originalDateTime.ToString("h:mm tt") == task.TimeOfDay && originalDateTime.ToString("MM/dd/yyyy") == task.Day && streamName == task.Name)
                    {
                        task.Name = streamName;
                        task.TimeOfDay = dateTime.ToString("h:mm tt");
                        task.Day = dateTime.ToString("MM/dd/yyyy");
                        break;
                    }
                }

                streamTasksList = SortStreamTasksList(streamTasksList);

                channel.StreamTasks = JsonConvert.SerializeObject(streamTasksList);
                await DataStore.SaveAsync(_homeHelperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", channel.Id } }, channel);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in UpdateStreamTask: " + e.Message);
                return false;
            }
        }

        public async Task<bool> RemoveStreamTask([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string streamName, DateTime originalDateTime, UserChannel channel)
        {
            try
            {
                var streamTasksList = JsonConvert.DeserializeObject<List<StreamTask>>(channel.StreamTasks);

                foreach (var task in streamTasksList)
                {
                    if (originalDateTime.ToString("h:mm tt") == task.TimeOfDay && originalDateTime.ToString("MM/dd/yyyy") == task.Day && streamName == task.Name)
                    {
                        streamTasksList.Remove(task);
                        break;
                    }
                }

                channel.StreamTasks = JsonConvert.SerializeObject(streamTasksList);
                await DataStore.SaveAsync(_homeHelperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", channel.Id } }, channel);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in RemoveStreamTask: " + e.Message);
                return false;
            }
        }

        private List<StreamTask> SortStreamTasksList(List<StreamTask> streamTasks)
        {
            try
            {
                var tasks = streamTasks.ToArray();
                SortUtil(tasks, 0, tasks.Length - 1);
                return tasks.ToList();
            }
            catch(Exception e)
            {
                Console.WriteLine("Error in SortStreamTasksList: " + e.Message);
                return null;
            }
        }

        private void SortUtil(StreamTask[] tasksArray, int leftBound, int rightBound) //quicksort
        {
            if (leftBound >= rightBound) return;

            var pivot = DateTime.ParseExact(tasksArray[tasksArray.Length - 1].TimeOfDay, "h:mm tt", CultureInfo.InvariantCulture);
            int counter = leftBound - 1;

            for (int i = leftBound; i < rightBound; i++)
            {
                if (DateTime.ParseExact(tasksArray[i].TimeOfDay, "h:mm tt", CultureInfo.InvariantCulture) <= pivot)
                {
                    counter++;

                    var temp = tasksArray[i];
                    tasksArray[i] = tasksArray[counter];
                    tasksArray[counter] = temp;
                }
            }

            var temp2 = tasksArray[counter + 1];
            tasksArray[counter + 1] = tasksArray[rightBound];
            tasksArray[rightBound] = temp2;

            counter++;

            SortUtil(tasksArray, leftBound, counter - 1);
            SortUtil(tasksArray, counter + 1, rightBound);
        }

        public int GetNumberOfFollowers(UserLogin userLogin)
        {
            if (userLogin.FollowedStudentsAndTutors == null) return 0;

            if (!userLogin.FollowedStudentsAndTutors.Contains('|')) return 1;

            var list = userLogin.FollowedStudentsAndTutors.Split('|');
            return list.Length;
        }

        public async Task ClearRecommendation ([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string id)
        {
            await DataStore.DeleteAsync<Recommendation>(_homeHelperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", id } });
        }

        public async Task DeleteStream([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string id)
        {
            await DataStore.DeleteAsync<UserArchivedStreams>(_homeHelperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", id } });
        }

        private int GetHoursAheadBasedOnTimeZone()
        {
            TimeZoneInfo localZone = TimeZoneInfo.Local;
            switch (localZone.DisplayName)
            {
                case "GMT-08:00": //PST
                    return 7;
                case "GMT-07:00": //MST
                    return 7;
            }

            return 0;
        }

        //public async Task<bool> SendStreamEmailToStudents([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, UserChannel channel)
        //{
        //    var listOfStudentEmails = await _homeHelperFunctions.GetUserLogins(storageConfig, QueryHeaders.AllStudentEmails, null);
        //    await _homeHelperFunctions.SendEmailToAnyEmailAsync(_homeHelperFunctions._streamworkEmailID, null, listOfStudentEmails.ToArray(), "A tutor has started a live-stream on StreamWork!", "Hey Student", null);
        //}
    }
}
