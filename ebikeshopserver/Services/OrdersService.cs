using System;
using System.Collections.Generic;
using ebikeshopserver.Exceptions;
using ebikeshopserver.Models.GlobalModels;
using ebikeshopserver.Models.Order;
using ebikeshopserver.Models.SellPosts;
using ebikeshopserver.Models.User;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ebikeshopserver.Services
{
    public class OrdersService
    {
        private readonly IMongoCollection<Order> _orders;
        private readonly IMongoCollection<SellPost> _sellPosts;
        private readonly IMongoCollection<User> _users;

        public OrdersService(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("ebike_shop_server");
            _orders = database.GetCollection<Order>("orders");
            _sellPosts = database.GetCollection<SellPost>("sellPosts");
            _users = database.GetCollection<User>("users");
        }

        public async Task<string> CreateNewOrderAsync(Order order)
        {
            decimal totalPrice = 0;

            foreach (var item in order.Items)
            {
                var sellPost = await _sellPosts.Find(sp => sp._id.ToString() == item.Key).FirstOrDefaultAsync();
                if (sellPost != null)
                {
                    decimal discountAmount = sellPost.Price * (decimal)sellPost.CurrentDiscount;
                    decimal discountedPricePerItem = sellPost.Price - discountAmount;

                    totalPrice += discountedPricePerItem * item.Value;

                    OrderSellPost frozenSellPost = new OrderSellPost
                    {
                        _id = sellPost._id.ToString(),
                        SellerId = sellPost.SellerId,
                        Title = sellPost.Title,
                        Price = sellPost.Price - discountAmount,
                        CurrentDiscount = sellPost.CurrentDiscount,
                        Quantity = item.Value
                    };
                    order.FrozenSellPost.Add(frozenSellPost);
                }
                else
                {
                    throw new NoPostsFoundException($"Sell post with ID {item.Key} not found.");
                }
            }

            order.TotalPrice = totalPrice;
            order.OrderTime = DateTime.Now;

            await _orders.InsertOneAsync(order);
            return order._id.ToString();
        }


        public async Task<List<Order>> GetOrdersByUserIdAsync(string userId) 
        {
            var user = await _users.Find(u => u.Id.ToString() == userId).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new UserNotFoundException($"User {userId} has not been found.");
            }
            var orders = await _orders.Find(order => order.UserId == userId).ToListAsync();
            if (orders == null)
            {
                throw new OrderNotFoundException($"No orders have been found for user {userId}.");
            }

            return orders;
        }

        public async Task<Order> GetOrderByIdAsync(string orderId) 
        {
            var order = await _orders.Find(o => o._id.ToString() == orderId).FirstOrDefaultAsync();
            if (order == null)
            {
                throw new OrderNotFoundException($"Order with ID {orderId} not found.");
            }
            return order;
        }

        public async Task<List<Order>> GetOrdersByDateRangeAsync(DateTime start, DateTime end, string userId)
        {
            var dateFilter = Builders<Order>.Filter.Gte(o => o.OrderTime, start) & Builders<Order>.Filter.Lte(o => o.OrderTime, end);
            var userFilter = Builders<Order>.Filter.Eq(o => o.UserId, userId);
            var combinedFilter = Builders<Order>.Filter.And(dateFilter, userFilter);

            var orderList = await _orders.Find(combinedFilter).ToListAsync();
            if (orderList == null)
            {
                throw new NoPostsFoundException("No posts at this date range have been found.");
            }

            return orderList;
        }

    }
}

