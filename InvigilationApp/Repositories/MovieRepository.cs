using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using InvigilationApp.Interfaces;
using InvigilationApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Xabe.FFmpeg;

namespace InvigilationApp.Repositories
{
    public class MovieRepository : IMovieRepository
    {

        private readonly IMovieService _movieService;
        private static readonly string ConnectionString = Secrets.BlobConnectionString;
        public static string BlobContainer = "movies";

        public MovieRepository(IMovieService movieService)
        {
            _movieService = movieService;
        }



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

        public async Task<FrameStats> GetMovieStats(string movieName)
        {
            var isMovieDownloaded = _movieService.DownloadMovie(movieName);
            var result = new FrameStats();

            if (isMovieDownloaded)
            {
                var path = Directory.GetCurrentDirectory();
                var input_path = Path.Combine(path, "downloads", $"{movieName}");
                // var output_path = Path.Combine(path, "uploads", $"{movieName}");
                

                try
                {
                    FFmpeg.SetExecutablesPath(Path.Combine(path, "ffmpeg"));

                    var info = await FFmpeg.GetMediaInfo(input_path);
                    var duration = info.Duration;

                    // Iterujemy po filmie
                    var samplingRate = 5;
                    for (int sec = 0; sec < duration.Seconds; sec += samplingRate)
                    {
                        var output_path = Path.Combine(path, "uploads", $"snapshot_{sec}.png");

                        var conversion = await FFmpeg.Conversions.FromSnippet.Snapshot(
                            input_path, 
                            output_path, 
                            TimeSpan.FromSeconds(sec));

                        var res = conversion.Start();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                // Weź wszystkie obrazki z lokalizacji --> Przeanalizuj
                try
                {

                }
                catch (Exception e)
                {

                }




                // var conversion = await FFmpeg.Conversions.FromSnippet.Snapshot(
                //     input_path, output_path, TimeSpan.FromSeconds(2));
                // var res = await conversion.Start();

                // var info = await FFmpeg.GetMediaInfo(input_path);

                // var videoStream = info.VideoStreams.First()
                //     .SetCodec(VideoCodec.h264)
                //     .SetSize(VideoSize.Hd480);
                //
                // await Conversion.New()
                //     .AddStream(videoStream)
                //     .SetOutput(output_path)
                //     .Start();

                using (var fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    var frames = _movieService.GetFramesFromFile(fs);
                }

            }

            // var path =
            //     $"C:\\Users\\adamk\\OneDrive\\Pulpit\\KODOWANIE\\Hackatony\\CodeNight\\InvigilationApp\\uploads\\7-min.PNG";
            //
            // var result = new FrameStats();
            // // Open the stream and read it back.
            // using (var fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None))
            // {
            //     result = _movieService.GetFrameStats(fs);
            // }

            return result;
            // return Task.FromResult<IList<FrameStats>>(stats);
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
