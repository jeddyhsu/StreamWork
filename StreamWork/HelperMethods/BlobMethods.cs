using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace StreamWork.HelperMethods
{
    public static class BlobMethods
    {
        private static readonly string _blobconnectionString = "DefaultEndpointsProtocol=https;AccountName=streamworkblob;AccountKey=//JfVlcPLOyzT3vRHxlY1lJ4NUpduVfiTmuHJHK1u/0vWzP8V5YHPLkPPGD2PVxEwTdNirqHzWYSk7c2vZ80Vg==;EndpointSuffix=core.windows.net";

        public static string SaveImageIntoBlobContainer(IFormFile file, string reference, int width, int height) //Saves Images
        {
            CloudStorageAccount cloudStorage = CloudStorageAccount.Parse(_blobconnectionString);
            CloudBlobClient blobClient = cloudStorage.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference("streamworkblobcontainer");
            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(reference);
            blockBlob.DeleteIfExists();

            var encoder = new PngEncoder();

            using (var stream = file.OpenReadStream())
            {
                using (var output = new MemoryStream())
                using (Image image = Image.Load(stream))
                {
                    int currWidth = image.Width;
                    int currHeight = image.Height;
                    if ((float)width / height > (float)currWidth / currHeight) // image is too tall relative to its width
                    {
                        int resizeHeight = (int)((float)currHeight / currWidth * width);
                        image.Mutate(i => i.Resize(width, resizeHeight).Crop(new Rectangle(0, Math.Abs(resizeHeight - height) / 2, width - 1, height - 1)));
                    }
                    else // image is too wide relative to its height, or perfect
                    {
                        int resizeWidth = (int)((float)currWidth / currHeight * height);
                        image.Mutate(i => i.Resize(resizeWidth, height).Crop(new Rectangle(Math.Abs(resizeWidth - width) / 2, 0, width - 1, height - 1)));
                    }
                     
                    image.Save(output, encoder);
                    output.Position = 0;
                    blockBlob.UploadFromStream(output);
                }
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
    }
}
