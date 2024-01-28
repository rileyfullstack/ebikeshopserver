using System;
using MongoDB.Bson;
using MongoDB.Driver;
using ebikeshopserver.Models.User;
using ebikeshopserver.Exceptions;
using ebikeshopserver.Utils;
using ebikeshopserver.Models.GlobalModels;
using System.Security.Authentication;

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
            if (newUser.Role != UserRole.User && newUser.Role != UserRole.Seller) //Admins will only be set menually
            {
                throw new BadRoleException("The role in the user that has been requested to be created is invalid.");
            }
            newUser.Password = PasswordHelper.GeneratePassword(newUser.Password);
            await _users.InsertOneAsync(newUser);
            return new { newUser.Id, newUser.FirstName, newUser.Email };
        }

        public async Task<List<User>> GetUsersAsync()
        {
            var builder = Builders<User>.Projection;
            var projection = builder.Exclude("Password");
            List<User> allUsers = await _users.Find(_ => true).Project<User>(projection).ToListAsync();
            return allUsers;
        }

        public async Task<User> GetUserAsync(string userId)
        {
            var builder = Builders<User>.Projection;
            var projection = builder.Exclude("Password");
            User? specificUser = await _users.Find(u => u.Id.ToString() == userId).Project<User>(projection).FirstOrDefaultAsync();
            if (specificUser == null)
            {
                throw new UserNotFoundException(userId);
            }
            return specificUser;
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var result = await _users.DeleteOneAsync(u => u.Id.ToString() == userId);
            if (result.DeletedCount == 0)
            {
                throw new UserNotFoundException("The user to delete wasn't found.");
            }
            return true;
        }

        public async Task<User> EditUserAsync(string userId, User updatedUser)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, new ObjectId(userId));

            var update = Builders<User>.Update
                .Set(u => u.FirstName, updatedUser.FirstName)
                .Set(u => u.LastName, updatedUser.LastName)
                .Set(u => u.Email, updatedUser.Email)
                .Set(u => u.UserAddress, updatedUser.UserAddress)
                .Set(u => u.PhoneNumber, updatedUser.PhoneNumber)
                .Set(u => u.ProfilePicture, updatedUser.ProfilePicture);

            var result = await _users.UpdateOneAsync(filter, update);

            // Check if the update was successful
            if (result.MatchedCount == 0)
            {
                throw new UserNotFoundException(userId);
            }

            updatedUser.Password = "";
            return updatedUser;
        }

        public async Task<bool> DeleteAddressAsync(string userId, string addressId) 
        {
            User? requestedUser = await _users.Find(u => u.Id.ToString() == userId).FirstOrDefaultAsync();
            if (requestedUser == null)
            {
                throw new UserNotFoundException(userId);
            }
            Address? addressToRemove = requestedUser.UserAddress.FirstOrDefault(a => a.AddressId.ToString() == addressId);
            if (addressToRemove == null)
            {
                throw new AddressNotFoundException(addressId);
            }

            requestedUser.UserAddress = requestedUser.UserAddress.Where(a => a.AddressId.ToString() != addressId).ToArray(); //gets all the addresss that arent the one being deleted

            await _users.ReplaceOneAsync(u => u.Id == requestedUser.Id, requestedUser); //Replaces the original user with a new one without the new address

            return true;
        }

        public async Task<User> LoginAsync(LoginModel loginModel)
        {
            var builder = Builders<User>.Projection;
            var userLogin = await _users.Find(u => u.Email == loginModel.Email).FirstOrDefaultAsync();
            if (userLogin == null)
            {
                throw new AuthenticationException("Login failed (User with recived email does not exist)");
            }
            if (!PasswordHelper.VerifyPassword(userLogin.Password, loginModel.Password))
            {
                throw new AuthenticationException("Login failed (Entered password is wrong.)");
            }

            return userLogin;
        }
    }
}

