using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace InvigilationApp.Interfaces
{
    public interface IMovieRepository
    {
        public Task<bool> UploadNewMovie(IFormFile file);
    }
}
