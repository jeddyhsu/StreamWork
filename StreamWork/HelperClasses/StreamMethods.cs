using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.Core;
using StreamWork.DataModels;

namespace StreamWork.HelperClasses
{
    public class StreamMethods
    {
        readonly HomeMethods _homeHelperFunctions = new HomeMethods();

        public async Task<bool> UserHasViewedStream([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string username, string tutorName)
        {
            return (await DataStore.GetListAsync<View>(_homeHelperFunctions._connectionString, storageConfig.Value, QueryHeaders.GetViewsWithViewerAndChannelSince.ToString(),
                new List<string> { username, tutorName, DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK") })).Count > 0;
        }

        public async Task<bool> UserHasViewedArchivedStream([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string username, string streamId)
        {
            return (await DataStore.GetListAsync<View>(_homeHelperFunctions._connectionString, storageConfig.Value, QueryHeaders.GetViewsWithViewerAndStreamId.ToString(),
                new List<string> { username, streamId })).Count > 0;
        }

        public async Task IncrementChannelViews([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string viewer, string username)
        {
            if (await UserHasViewedStream(storageConfig, viewer, username))
                return;

            UserChannel channel = (await _homeHelperFunctions.GetUserChannels(storageConfig, QueryHeaders.GetUserChannelWithUsername, username))[0];
            channel.Views++;
            await DataStore.SaveAsync(_homeHelperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", channel.Id } }, channel);

            View view = new View
            {
                Id = Guid.NewGuid().ToString(),
                Viewer = viewer,
                Channel = username,
                Date = DateTime.UtcNow
            };
            await DataStore.SaveAsync(_homeHelperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", view.Id } }, view);
        }

        public async Task IncrementArchivedVideoViews([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string viewer, string id)
        {
            if (await UserHasViewedArchivedStream(storageConfig, viewer, id))
                return;

            UserArchivedStreams archivedStream = (await _homeHelperFunctions.GetArchivedStreams(storageConfig, QueryHeaders.GetArchivedStreamsWithStreamId, id))[0];
            archivedStream.Views++;
            await DataStore.SaveAsync(_homeHelperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", archivedStream.Id } }, archivedStream);

            View view = new View
            {
                Id = Guid.NewGuid().ToString(),
                Viewer = viewer,
                Channel = archivedStream.Username,
                StreamId = id,
                Date = DateTime.UtcNow
            };
            await DataStore.SaveAsync(_homeHelperFunctions._connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", view.Id } }, view);
        }

        public string GetCorrespondingSubjectThumbnail(string subject)
        {
            Hashtable table = new Hashtable();
            table.Add("Mathematics", "/images/ChatAssets/Math.png");
            table.Add("Science", "/images/ChatAssets/Science.png");
            table.Add("Business", "/images/ChatAssets/Business.png");
            table.Add("Engineering", "/images/ChatAssets/Engineering.png");
            table.Add("Law", "/images/ChatAssets/Law.png");
            table.Add("Art", "/images/ChatAssets/Art.png");
            table.Add("Humanities", "/images/ChatAssets/Humanities.png");
            table.Add("Other", "/images/ChatAssets/Other.png");


            return (string)table[subject];
        }
    }
}
