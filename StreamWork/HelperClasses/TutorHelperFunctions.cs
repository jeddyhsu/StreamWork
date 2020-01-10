﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
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
        readonly HomeHelperFunctions _helperFunctions = new HomeHelperFunctions();
        private const int NumberOfDays = 11;

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
            var allArchivedStreams = await _helperFunctions.GetArchivedStreams(storageConfig, QueryHeaders.UserArchivedVideos, user);
            var userChannel = await _helperFunctions.GetUserChannels(storageConfig, QueryHeaders.CurrentUserChannel, user);
            foreach (var stream in allArchivedStreams)
            {
                stream.ProfilePicture = profilePicture;
                await DataStore.SaveAsync(_helperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", stream.Id } }, stream);
            }
            userChannel[0].ProfilePicture = profilePicture;
            await DataStore.SaveAsync(_helperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userChannel[0].Id } }, userChannel[0]);
        }

        public async Task<string> GetChatSecretKey([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string user)
        {
            var userChannel = await _helperFunctions.GetUserChannels(storageConfig, QueryHeaders.CurrentUserChannel, user);
            var ids = userChannel[0].ChatId.Split("|");
            var encodedUrl = HttpUtility.UrlEncode(Convert.ToBase64String(_helperFunctions.hmacSHA256("/box/?boxid=" + 829647 + "&boxtag=oq4rEn&tid=" + ids[0] + "&tkey=" + ids[1] + "&nme=" + userChannel[0].Username, "3O08UU-OtQ_rycx3")));
            var finalString = "https://www6.cbox.ws" + "/box/?boxid=" + 829647 + "&boxtag=oq4rEn&tid=" + ids[0] + "&tkey=" + ids[1] + "&nme=" + userChannel[0].Username + "&sig=" + encodedUrl;
            return finalString;
        }

        public List<Day> GetTutorStreamSchedule(UserChannel channel)
        {
            var todaysDate = DateTime.Now;

            Schedule schedule = new Schedule();

            schedule.Days.Add(todaysDate.ToShortDateString(), new Day(todaysDate));
            schedule.Days.Add(todaysDate.AddDays(1.0).ToShortDateString(), new Day(todaysDate.AddDays(1.0)));
            schedule.Days.Add(todaysDate.AddDays(2.0).ToShortDateString(), new Day(todaysDate.AddDays(2.0)));
            schedule.Days.Add(todaysDate.AddDays(3.0).ToShortDateString(), new Day(todaysDate.AddDays(3.0)));
            schedule.Days.Add(todaysDate.AddDays(4.0).ToShortDateString(), new Day(todaysDate.AddDays(4.0)));
            schedule.Days.Add(todaysDate.AddDays(5.0).ToShortDateString(), new Day(todaysDate.AddDays(5.0)));
            schedule.Days.Add(todaysDate.AddDays(6.0).ToShortDateString(), new Day(todaysDate.AddDays(6.0)));
            schedule.Days.Add(todaysDate.AddDays(7.0).ToShortDateString(), new Day(todaysDate.AddDays(7.0)));
            schedule.Days.Add(todaysDate.AddDays(8.0).ToShortDateString(), new Day(todaysDate.AddDays(8.0)));
            schedule.Days.Add(todaysDate.AddDays(9.0).ToShortDateString(), new Day(todaysDate.AddDays(9.0)));
            schedule.Days.Add(todaysDate.AddDays(10.0).ToShortDateString(), new Day(todaysDate.AddDays(10.0)));
            schedule.Days.Add(todaysDate.AddDays(11.0).ToShortDateString(), new Day(todaysDate.AddDays(11.0)));

            FormatTutorStreamSchedule(schedule, channel);

            ICollection collection = schedule.Days.Keys;
            List<Day> days = new List<Day>();

            foreach(string key in collection)
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
                var day = schedule.Days[streamTask.Day];
                day.StreamTaskList.Add(streamTask);
            }
        }

        public async Task<bool> AddStreamTask([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string streamName, DateTime dateTime, UserChannel channel)
        {
            if(channel.StreamTasks != null)
            {
                var streamTaskList = JsonConvert.DeserializeObject<List<StreamTask>>(channel.StreamTasks);
                streamTaskList.Add(new StreamTask(streamName, dateTime.ToShortTimeString(), dateTime.ToShortDateString()));
                var serialize = JsonConvert.SerializeObject(streamTaskList);
                channel.StreamTasks = serialize;
                await DataStore.SaveAsync(_helperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", channel.Id } }, channel);
                return true;
            }
            else
            {
                List<StreamTask> streamTasks = new List<StreamTask>();
                streamTasks.Add(new StreamTask(streamName, dateTime.ToShortTimeString(), dateTime.ToShortDateString()));
                var serialize = JsonConvert.SerializeObject(streamTasks);
                channel.StreamTasks = serialize;
                await DataStore.SaveAsync(_helperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", channel.Id } }, channel);
                return true;
            }
        }
    }
}
