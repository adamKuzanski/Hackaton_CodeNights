using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InvigilationApp.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace InvigilationApp.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private static readonly string ConnectionString = Secrets.BlobConnectionString;
        public static string BlobContainer = "movies";

        public async Task<bool> UploadNewMovie(IFormFile movie)
        {

            var storageAccount = CloudStorageAccount.Parse(ConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var cloudBlobContainer = blobClient.GetContainerReference(BlobContainer);

            if (await cloudBlobContainer.CreateIfNotExistsAsync())
            {
                await cloudBlobContainer.SetPermissionsAsync(
                    new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Off
                    });
            }

            var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(movie.FileName);
            cloudBlockBlob.Properties.ContentType = movie.ContentType;

            await cloudBlockBlob.UploadFromStreamAsync(
                movie.OpenReadStream());

            return true;
        }

    }
}
