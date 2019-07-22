﻿using System.Collections.Generic;
using System.Linq;

namespace CookieSample.Data
{
    public class UserStore
    {
        private static List<User> _users = new List<User>
        {
            new User
            {
                Id = 1,
                Name = "alice",
                Password = "alice",
                Email = "alice@gmail.com",
                PhoneNumber = "18800000001"
            },
            new User
            {
                Id = 1,
                Name = "bob",
                Password = "bob",
                Email="bob@gmail.com",
                PhoneNumber ="18800000002"
            }
        };

        public User FindUser(string userName,string password)
        {
            return _users.FirstOrDefault(f => f.Name == userName && f.Password == password);
        }
    }
}
