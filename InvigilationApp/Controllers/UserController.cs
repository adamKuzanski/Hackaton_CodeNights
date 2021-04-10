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
        // GET: api/<UserController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

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

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
