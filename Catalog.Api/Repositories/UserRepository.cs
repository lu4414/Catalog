using System.Collections.Generic;
using System.Linq;
using Catalog.Api.Models;

namespace Catalog.Api.Repositories
{
    public static class UserRepository
    {
        public static User Get(string username, string password)
        {
            var users = new List<User>();
            users.Add(new User { Id = 1, Username = "admin", Password = "boss", Role = "admin" });
            users.Add(new User { Id = 2, Username = "player", Password = "player", Role = "player" });
            return users.Where(x => x.Username.ToLower() == username.ToLower() && x.Password == x.Password).FirstOrDefault();
        }
    }
}