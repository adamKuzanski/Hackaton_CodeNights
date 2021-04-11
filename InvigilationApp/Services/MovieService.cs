using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using InvigilationApp.Interfaces;
using InvigilationApp.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Newtonsoft.Json;
using RestSharp;

namespace InvigilationApp.Services
{
    public class MovieService: IMovieService
    {
        private const double Threshold = 0.6;

        public FrameStats GetFrameStats(Stream imageStream)
        {
            var predictions = GetPredictions(imageStream).Result;
            return ProceedPredictions(predictions);
        }

        public bool DownloadMovie(string movieName)
        {
            using var client = new WebClient();
            client.DownloadFile(Secrets.BlobUrl + movieName, $"downloads\\{movieName}");

            return true;
        }

        public List<string> GetFramesFromFile(FileStream fs)
        {
            // to do: Extract frames from video
            throw new NotImplementedException();
        }

        private FrameStats ProceedPredictions(Predictions predictions)
        {
            var frameStats = new FrameStats();
            foreach (var prediction in predictions.predictions)
            {
                if (prediction.probability > Threshold)
                {
                    if (prediction.tagName.Equals("Car"))
                    {
                        frameStats.NbOfCars++;
                    }
                    else if (prediction.tagName.Equals("Person_BadMask"))
                    {
                        frameStats.NbOfPeopleWithOutMask++;
                        frameStats.NbOfPeopleOnImage++;
                    }
                    else if (prediction.tagName.Equals("Person_GoodMask"))
                    {
                        frameStats.NbOfPeopleWithMask++;
                        frameStats.NbOfPeopleOnImage++;
                    }
                    else if (prediction.tagName.Equals("Person_Rear"))
                    {
                        frameStats.NbOfPeopleOnImage++;
                    }
                }
            }

            return frameStats;
        }

        private async Task<Predictions> GetPredictions(Stream imageStream)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Prediction-key", Secrets.NNPredictionKey);

            byte[] imageData;
            await using (var memoryStream = new MemoryStream())
            {
                await imageStream.CopyToAsync(memoryStream);
                imageData = memoryStream.ToArray();
            }

            HttpResponseMessage response;
            using (var content = new ByteArrayContent(imageData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(Secrets.NNUrlAddress, content);
            }

            var resultJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Predictions>(resultJson);
        }
    }
}
