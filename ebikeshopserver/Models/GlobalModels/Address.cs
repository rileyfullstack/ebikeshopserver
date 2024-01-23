using System;
namespace ebikeshopserver.Models.GlobalModels
{
	public class Address
	{
        public string? State { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public int HouseNumber { get; set; }
        public int Zip { get; set; }

        public Address(string state, string country, string city, string street, int houseNumber, int zip = 0)
        {
            State = state;
            Country = country;
            City = city;
            Street = street;
            HouseNumber = houseNumber;
            Zip = zip;
        }
    }
}

