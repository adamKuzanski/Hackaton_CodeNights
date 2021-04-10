using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InvigilationApp.Interfaces;
using InvigilationApp.Models;
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

        public Task<IList<FrameStats>> GetMovieStats(string movieName)
        {
            var rand = new Random();

            var stats = new List<FrameStats>();
            for (var i = 0; i < 20; i++)
            {
                var temp_stats = new FrameStats();
                temp_stats.FrameNb = i;
                temp_stats.NbOfCars = 2 - rand.Next(-2, 1);
                temp_stats.NbOfCyclers = 3 - rand.Next(-3, 1);
                temp_stats.NbOfPeopleOnImage = i * 2;
                temp_stats.NbOfPeopleWithMask = i + 3;
                temp_stats.NbOfPeopleWithOutMask =
                    temp_stats.NbOfPeopleOnImage - temp_stats.NbOfPeopleWithMask - rand.Next(0, 5);

                temp_stats.NbOfPeopleWithOutMask = Math.Abs(temp_stats.NbOfPeopleWithOutMask);
                stats.Add(temp_stats);
            }

            return Task.FromResult<IList<FrameStats>>(stats);
        }

        public async Task<List<string>> GetAllMovieNames()
        {
            var storageAccount = CloudStorageAccount.Parse(ConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var cloudBlobContainer = blobClient.GetContainerReference(BlobContainer);

            BlobContinuationToken continuationToken = null; //start at the beginning
            var results = new List<IListBlobItem>();
            do
            {
                var response = await cloudBlobContainer.ListBlobsSegmentedAsync(continuationToken);
                continuationToken = response.ContinuationToken;
                results.AddRange(response.Results);
            }

            while (continuationToken != null); //when this is null again, we've reached the end

            return results.Cast<CloudBlockBlob>().Select(b => b.Name).ToList();
        }
    }
}
