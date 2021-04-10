using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvigilationApp.Models
{
    public class User
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class UserLogin
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserServer: User
    {
        public string Token { get; set; }
    }
}
