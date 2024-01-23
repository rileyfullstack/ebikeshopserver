using System;
using MongoDB.Bson;
using System.Net;
using System.Xml.Linq;
using ebikeshopserver.Models.GlobalModels;

namespace ebikeshopserver.Models.User
{
	public class User
	{
        public ObjectId Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public Image ProfilePicture { get; set; }
        public Address[] UserAddress { get; set; }
        public string Role { get; set; } //Either user, seller, or admin
        public DateTime UserCreationDate { get; set; }

        public User(string firstName, string lastName, string email,
                string password, string phoneNumber, Image profilePicture,
                Address[] userAddress, string role)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
            PhoneNumber = phoneNumber;
            ProfilePicture = profilePicture;
            UserAddress = userAddress;
            Role = role;
            UserCreationDate = DateTime.Now;
        }
    }
}

