using System;
using System.ComponentModel.DataAnnotations;
using ebikeshopserver.Models.GlobalModels;

namespace ebikeshopserver.Models.Order
{
	public class OrderSellPost
	{
        [Required]
        public string _id { get; set; }

        [Required]
        public string SellerId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public decimal Price { get; set; }

        public double CurrentDiscount { get; set; }

        public int Quantity { get; set; }
    }
}

