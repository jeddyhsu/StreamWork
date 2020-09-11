using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace StreamWork.HelperMethods
{
    public static class BlobMethods
    {
        private static readonly string _blobconnectionString = "DefaultEndpointsProtocol=https;AccountName=streamworkblob;AccountKey=//JfVlcPLOyzT3vRHxlY1lJ4NUpduVfiTmuHJHK1u/0vWzP8V5YHPLkPPGD2PVxEwTdNirqHzWYSk7c2vZ80Vg==;EndpointSuffix=core.windows.net";

        public static async Task<string> SaveImageIntoBlobContainer(IFormFile file, string reference, int width, int height) //Saves Images
        {
            var requestOption = new BlobRequestOptions()
            {
                ParallelOperationThreadCount = 8,
                SingleBlobUploadThresholdInBytes = 10 * 1024 * 1024
            };

            CloudStorageAccount cloudStorage = CloudStorageAccount.Parse(_blobconnectionString);
            CloudBlobClient blobClient = cloudStorage.CreateCloudBlobClient();
            blobClient.DefaultRequestOptions = requestOption;
            CloudBlobContainer blobContainer = blobClient.GetContainerReference("streamworkblobcontainer");
            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(reference);
            blockBlob.DeleteIfExists();

            using (var stream = file.OpenReadStream())
            {
                await blockBlob.UploadFromStreamAsync(stream);
            }

            return blockBlob.Uri.AbsoluteUri + "?" + DateTime.Now;
        }

        public static string SaveFileIntoBlobContainer(string reference, string content) //Tutor Section TXT Files
        {
            CloudStorageAccount cloudStorage = CloudStorageAccount.Parse(_blobconnectionString);
            CloudBlobClient blobClient = cloudStorage.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference("streamworksectionsandtopics");
            CloudBlockBlob blob = blobContainer.GetBlockBlobReference(reference);
            blob.DeleteIfExists();

            blob.UploadText(content);

            return blob.Uri.AbsoluteUri;
        }

        public static CloudBlockBlob GetBlockBlob(string reference)
        {
            CloudStorageAccount cloudStorage = CloudStorageAccount.Parse(_blobconnectionString);
            CloudBlobClient blobClient = cloudStorage.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference("streamworksectionsandtopics");
            CloudBlockBlob blob = blobContainer.GetBlockBlobReference(reference);
            return blob;
        }

        public static string Log(string reference, string content) //Tutor Section TXT Files
        {
            CloudStorageAccount cloudStorage = CloudStorageAccount.Parse(_blobconnectionString);
            CloudBlobClient blobClient = cloudStorage.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference("streamworksectionsandtopics");
            CloudBlockBlob blob = blobContainer.GetBlockBlobReference(reference);
            blob.DeleteIfExists();

            blob.UploadText(content);

            return blob.Uri.AbsoluteUri;
        }
    }
}
