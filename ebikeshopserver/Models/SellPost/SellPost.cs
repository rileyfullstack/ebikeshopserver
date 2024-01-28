using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;

namespace ebikeshopserver.Models.SellPost
{
	public class SellPost
	{
		public ObjectId PostID { get; set; }

		[Required]
		public string SellerId { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public Type Type { get; set; }
		public SubType SubType { get; set; }
		public string CatagoryId { get; set; }
		public string SubcatagoryId { get; set; }
		public Decimal128 Price { get; set; }


        public SellPost()
		{
		}
	}
}

