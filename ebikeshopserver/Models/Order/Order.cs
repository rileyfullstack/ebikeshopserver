using System;
using MongoDB.Bson;
using ebikeshopserver.Models.SellPosts;
using ebikeshopserver.Models.GlobalModels;
using System.ComponentModel.DataAnnotations;

namespace ebikeshopserver.Models.Order
{
	public class Order
	{
		public ObjectId _id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public Dictionary<string, int> Items { get; set; }

        [Required]
        public DateTime OrderTime { get; set; } 

        public decimal? TotalPrice { get; set; }

        [Required]
        public Address Address { get; set; }
    }
}