using System;
using MongoDB.Bson;
using MongoDB.Driver;
using ebikeshopserver.Models.User;
using ebikeshopserver.Exceptions;
using ebikeshopserver.Utils;

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

        public async Task<object> CreateUserAsync(User newUser)
        {
            var existingUser = await _users.Find(u => u.Email == newUser.Email).FirstOrDefaultAsync();
            if (existingUser != null)
            {
                throw new UserAlreadyExistsException("User with this email already exists.");
            }

            newUser.Password = PasswordHelper.GeneratePassword(newUser.Password);
            await _users.InsertOneAsync(newUser);
            return new { newUser.Id, newUser.FirstName, newUser.Email };
        }
    }
}

