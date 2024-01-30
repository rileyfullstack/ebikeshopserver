using System;
using System.ComponentModel.DataAnnotations;
using ebikeshopserver.Models.GlobalModels;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ebikeshopserver.Models.SellPost
{
	public class SellPost
	{
        [Required]
        public ObjectId _id { get; set; }

		[Required]
		public string SellerId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Catagory { get; set; }

        [Required]
        public string SubCatagory { get; set; }

        [Required]
        public string CatagoryId { get; set; }

        [Required]
        public string SubcatagoryId { get; set; }

        [Required]
        public decimal Price { get; set; }

        public Image[] Image { get; set; }

        public double CurrentDiscount { get; set; }

        public Dictionary<string,string> Specifications { get; set; }

        [Required]
        public int ItemsLeft { get; set; }

        [Required]
        public SellPostStatus Status { get; set; }

        public DateTime PostCreationDate { get; }

        public DateTime? LastUpdatedDate { get; set; }
	}
}

