using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace InvigilationApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IHostEnvironment _hostingEnvironment;

        public MovieController(IWebHostEnvironment environment)
        {
            _hostingEnvironment = environment;
        }


        [HttpPost("uploadMovie")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadMovie(IFormFile file)
        {
            var uploads = Path.Combine(_hostingEnvironment.ContentRootPath, "uploads");
            var filePath = Path.Combine(uploads, file.FileName);

            await using (Stream fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
            }

            var message = $"File named {file.FileName} has been uploaded";
            return Ok(message);
        }
    }
}
