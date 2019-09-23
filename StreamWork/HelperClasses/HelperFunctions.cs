using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.Core;
using StreamWork.DataModels;
using StreamWork.Models;

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
        public async Task<string> SaveIntoBlobContainer(IFormFile file, [FromServices] IOptionsSnapshot<StorageConfig> storageConfig, string user, string reference)
        { 
            //Connects to blob storage and saves thumbnail from user
            CloudStorageAccount cloudStorage = CloudStorageAccount.Parse(_blobconnectionString);
            CloudBlobClient blobClient = cloudStorage.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference("streamworkblobcontainer");
            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(reference);

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

            return blockBlob.Uri.AbsoluteUri;
        }

        public async Task<bool> SaveDonation([FromServices] IOptionsSnapshot<StorageConfig> storageConfig, Donation donation) {
            await DataStore.SaveAsync(_connectionString, storageConfig.Value, new Dictionary<string, object> { { "Id", donation.Id } }, donation);
            return true;
        }

        //sends to any email from streamworktutor@gmail.com provided the 'to' 'subject' & 'body'
        public void SendEmailToAnyEmail(string to, string subject, string body)
        {
            //For localhost use smtp.gmail.com
            SmtpClient client = new SmtpClient("smtp.streamwork.live", 587)
            {
                Credentials = new NetworkCredential("streamworktutor@gmail.com", "STREAMW0RK0!"),
                EnableSsl = true
            };

            client.Send("streamworktutor@gmail.com", to, subject, body);
        }

        public string CreateUri(string username)
        {
            var uriBuilder = new UriBuilder("http://localhost:58539/Home/ChangePassword");
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["username"] = username;
            uriBuilder.Query = query.ToString();
            return uriBuilder.ToString();
        }
    }
}
