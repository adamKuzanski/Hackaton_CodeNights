using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using InvigilationApp.Models;
using Microsoft.AspNetCore.Http;

namespace InvigilationApp.Interfaces
{
    public interface IMovieService
    {
        public FrameStats GetFrameStats(Stream imageStream);
    }
}
