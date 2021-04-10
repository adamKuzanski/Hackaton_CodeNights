using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InvigilationApp.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InvigilationApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        // POST api/user/registration
        [HttpPost("registration")]
        public async Task<IActionResult> PostRegisterUser([FromBody] User user)
        {
            return new OkObjectResult(user);
        }

        // POST api/user/login
        [HttpPost("login")]
        public async Task<IActionResult> PostLogInUser([FromBody] UserLogin user)
        {
            var userResponse = new UserServer
            {
                Email = user.Email,
                Password = user.Password,
                FirstName = "To do",
                LastName = "To do",
                Token = "fake-jwt-token"
            };
            return new OkObjectResult(userResponse);
        }
    }
}
