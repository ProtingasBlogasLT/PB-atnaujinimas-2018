using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PB.WebAPI.Models
{
    public class User : IDbModel
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string HashedPassword { get; set; }
        public string Token { get; set; }
    }
}
