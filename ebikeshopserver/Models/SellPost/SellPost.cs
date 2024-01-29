using System;
using System.ComponentModel.DataAnnotations;
using ebikeshopserver.Models.GlobalModels;
using MongoDB.Bson;

namespace ebikeshopserver.Models.SellPost
{
	public class SellPost
	{
        [Required]
        public ObjectId PostID { get; set; }

		[Required]
		public string SellerId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public Type Type { get; set; }

        [Required]
        public SubType SubType { get; set; }

        [Required]
        public string CatagoryId { get; set; }

        [Required]
        public string SubcatagoryId { get; set; }

        [Required]
        public Decimal128 Price { get; set; }

        public Image[] Image { get; set; }

        public double CurrentDiscount { get; set; }

        public Dictionary<string,string> Specifications { get; set; }

        [Required]
        public int ItemsLeft { get; set; }

        [Required]
        public SellPostStatus Status { get; set; }

        public DateTime PostCreationDate { get; }

        private DateTime? LastUpdatedDate;
	}
}

