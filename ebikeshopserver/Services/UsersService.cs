using System;
using MongoDB.Bson;
using MongoDB.Driver;
using ebikeshopserver.Models.User;

namespace ebikeshopserver.Services
{
    public class UsersService
    {
        private IMongoCollection<User> _users;

        public UsersService(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("ebike_shop_server");
            _users = database.GetCollection<User>("users");
        }
    }
}

