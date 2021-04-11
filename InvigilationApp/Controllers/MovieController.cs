using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using InvigilationApp.Interfaces;
using InvigilationApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.WindowsAzure.Storage.Blob.Protocol;

namespace InvigilationApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase 
    {
        private readonly IHostEnvironment _hostingEnvironment;
        private readonly IMovieRepository _movieRepository;

        public MovieController(IWebHostEnvironment environment, IMovieRepository movieRepository)
        {
            _hostingEnvironment = environment;
            _movieRepository = movieRepository;
        }


        [HttpPost("uploadMovie")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> PostUploadMovie(IFormFile file)
        {
            var uploads = Path.Combine(_hostingEnvironment.ContentRootPath, "uploads");
            var filePath = Path.Combine(uploads, file.FileName);

            var result = _movieRepository.UploadNewMovie(file);

            string message;
            if (await result)
            {
                message = $"OK: File named {file.FileName} has been uploaded";
                return Ok(message);
            }
            else
            {
                message = $"You FUCKED UP: File named {file.FileName} has been uploaded";
                return BadRequest(message);
            }
        }

        [HttpGet("analyseMove")]
        public async Task<IActionResult> GetAnalyseMovie(string movieName)
        {
            var stats = _movieRepository.GetMovieStats(movieName);

            string message; 
            var result = true;
            if (result)
            {
                message = $"OK: Stats sent";
                return Ok(stats);
            }
            else
            {
                message = $"You FUCKED UP";
                return BadRequest(stats);
            }
        }

        [HttpGet("allNames")]
        public async Task<IActionResult> GetAllMovieNames()
        {
            var namesList = await _movieRepository.GetAllMovieNames();

            string message;
            var result = true;
            if (result)
            {
                message = $"OK: Stats sent";
                return Ok(namesList);
            }
            else
            {
                message = $"You FUCKED UP";
                return BadRequest(namesList);
            }
        }


    }
}
