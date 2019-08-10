using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.Core;
using StreamWork.DataModels;
using StreamWork.Models;
using StreamWork.ViewModels;

namespace StreamWork
{
    public class HelperFunctions
    {
        public readonly string _connectionString = "Server=tcp:streamwork.database.windows.net,1433;Initial Catalog=StreamWork;Persist Security Info=False;User ID=streamwork;Password=arizonastate1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        public readonly string _blobconnectionString = "DefaultEndpointsProtocol=https;AccountName=streamworkblob;AccountKey=//JfVlcPLOyzT3vRHxlY1lJ4NUpduVfiTmuHJHK1u/0vWzP8V5YHPLkPPGD2PVxEwTdNirqHzWYSk7c2vZ80Vg==;EndpointSuffix=core.windows.net";

        //Gets set of userchannels with the query that you specify
        public async Task<List<UserChannel>> GetUserChannels([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string query, string user)
        {
            var channels = await DataStore.GetListAsync<UserChannel>(_connectionString, storageConfig.Value, query, new List<string> { user});
            return channels;
        }

        //Gets a set of archived streams with the query that you specify
        public async Task<List<UserArchivedStreams>> GetArchivedStreams([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string query, string user)
        {
            var archivedStreams = await DataStore.GetListAsync<UserArchivedStreams>(_connectionString, storageConfig.Value, query, new List<string> { user });
            return archivedStreams;
        }

        //Gets a set of user logins with the query that you specify
        public async Task<List<UserLogin>> GetUserLogins([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string query, string user)
        {
           var logins = await DataStore.GetListAsync<UserLogin>(_connectionString, storageConfig.Value, query, new List<string> { user });
           return logins;
        }

        //Gets user login info with the query that you specify
        public async Task<UserLogin> GetUserProfile([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string query, string user)
        {
            var logins = await DataStore.GetListAsync<UserLogin>(_connectionString, storageConfig.Value, query, new List<string> { user });
            return logins[0];
        }

        //Saves profilePicture into container on Azure
        public async Task<bool> SaveIntoBlobContainer(IFormFile file, string profileCaption, string profileParagraph, [FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string user)
        {
            var userProfile = await GetUserProfile(storageConfig, "CurrentUser", user);

            //Connects to blob storage and saves thumbnail from user
            CloudStorageAccount cloudStorage = CloudStorageAccount.Parse(_blobconnectionString);
            CloudBlobClient blobClient = cloudStorage.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference("streamworkblobcontainer");
            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(profileCaption);


            using (MemoryStream ms = new MemoryStream())
            {
                try
                {
                    await file.CopyToAsync(ms);
                    blockBlob.UploadFromByteArray(ms.ToArray(), 0, (int)file.Length);
                }
                catch (System.ObjectDisposedException e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            //Populates stream title and stream thumbnail url and saves it into sql database
            userProfile.ProfileCaption = profileCaption;
            userProfile.ProfilePicture = blockBlob.Uri.AbsoluteUri;
            userProfile.ProfileParagraph = profileParagraph;
            await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", userProfile.Id } }, userProfile);

            return true;
        }
    }
}
