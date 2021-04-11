using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InvigilationApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage.Blob;

namespace InvigilationApp.Interfaces
{
    public interface IMovieRepository
    {
        public Task<bool> UploadNewMovie(IFormFile file);
        public Task<List<FrameStats>> GetMovieStats(string movieName);

        public Task<IList<FrameStats>> GetRandomMovieStats(string movieName);
        public Task<List<string>> GetAllMovieNames();
    }
}
