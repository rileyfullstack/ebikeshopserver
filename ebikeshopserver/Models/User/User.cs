using System;
using MongoDB.Bson;
using System.Net;
using System.Xml.Linq;
using ebikeshopserver.Models.GlobalModels;
using System.ComponentModel.DataAnnotations;

namespace ebikeshopserver.Models.User
{
	public class User
	{
        public ObjectId Id { get; set; }

        [Required, StringLength(maximumLength: 32, MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required, StringLength(maximumLength: 32, MinimumLength = 2)]
        public string LastName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, StringLength(maximumLength: 32, MinimumLength = 8)]
        public string Password { get; set; }

        [Required, Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public Image ProfilePicture { get; set; }

        [Required]
        public Address[] UserAddress { get; set; }

        [Required]
        public UserRole Role { get; set; } //Either User, Seller, or Admin

        public DateTime UserCreationDate { get; set; }

        public User(string firstName, string lastName, string email,
                string password, string phoneNumber, Image profilePicture,
                Address[] userAddress, UserRole role)
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

